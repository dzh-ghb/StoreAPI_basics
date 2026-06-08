using Api.Common;
using Api.Data;
using Api.Model;
using Api.ModelDto;
using Api.Services.StorageS3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class PostgreSqlEfStorage : IStorage
{
    protected readonly AppDbContext dbContext;
    private readonly IFileStorageService fileStorage;

    public PostgreSqlEfStorage(AppDbContext dbContext, IFileStorageService fileStorage)
    {
        this.dbContext = dbContext;
        this.fileStorage = fileStorage;
    }

    #region ProductsInfrastructure

    public async Task<Product> AddProduct(ProductCreateDto productCreateDto)
    {
        Product item = new()
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            SpecialTag = productCreateDto.SpecialTag,
            Category = productCreateDto.Category,
            Price = productCreateDto.Price,
            Image = await fileStorage.UploadFileAsync(productCreateDto.Image)
            // Image = $"https://placehold.co/100" // демо-вариант с фейковым значением
        };

        // добавление в БД
        dbContext.Products.Add(item);
        dbContext.SaveChanges(); // id из БД будет добавлен в item после обработки операции Entity Framework

        return item;
    }

    public List<Product> GetAllProducts()
    {
        return dbContext.Products.ToList(); // БД.таблица.преобразовать_в_список()
    }

    public async Task<List<Product>> GetProductsWithPagination(int skip = 0, int take = 5)
    {
        int count = GetProductsCount();

        if (skip < 0 || skip > count - 1 || skip + take > count || take > count - skip)
        {
            return null;
        }

        return await dbContext.Products.OrderBy(i => i.Id).Skip(skip).Take(take).ToListAsync();
    }

    public Product GetProduct(int id)
    {
        return dbContext.Products.FirstOrDefault(x => x.Id == id);
    }

    public async Task<Product> UpdateProduct(int id, ProductUpdateDto productUpdateDto)
    {
        Product item = GetProduct(id);

        if (item == null)
        {
            return null;
        }

        item.Name = productUpdateDto.Name; // поле обязательное, проверка не нужна
        item.Description = productUpdateDto.Description;
        if (!String.IsNullOrEmpty(productUpdateDto.SpecialTag))
        {
            item.SpecialTag = productUpdateDto.SpecialTag;
        }
        if (!String.IsNullOrEmpty(productUpdateDto.Category))
        {
            item.Category = productUpdateDto.Category;
        }
        item.Price = productUpdateDto.Price;
        if (productUpdateDto.Image != null
            && productUpdateDto.Image.Length > 0)
        {
            await fileStorage.RemoveFileAsync(item.Image.Split('/').Last());
            item.Image = await fileStorage.UploadFileAsync(productUpdateDto.Image);
            // item.Image = $"https://placehold.co/200";
        }

        dbContext.Products.Update(item);
        dbContext.SaveChanges();

        return item;
    }

    public async Task<bool> RemoveProductAsync(int id)
    {
        Product item = GetProduct(id);

        if (item == null)
        {
            return false;
        }

        await fileStorage.RemoveFileAsync(item.Image.Split('/').Last());
        dbContext.Products.Remove(item);
        dbContext.SaveChanges();

        return true;
    }

    public int GetProductsCount()
    {
        return dbContext.Products.Count();
    }

    #endregion

    #region AuthInfrastructure

    public async Task<bool> AddUser(RegisterRequestDto registerRequestDto, UserManager<AppUser> userManager/*, RoleManager<IdentityRole> roleManager*/)
    {
        AppUser user = new AppUser
        {
            UserName = registerRequestDto.UserName,
            Email = registerRequestDto.Email,
            // NormalizedEmail = registerRequestDto.Email.ToUpper(),
            FirstName = registerRequestDto.UserName
        };

        // попытка создания юзера
        var result = await userManager.CreateAsync(user, registerRequestDto.Password);

        if (!result.Succeeded)
        {
            return false;
        }

        // определение указанной роли
        var role = registerRequestDto.Role.Equals(
            SharedData.Roles.Admin, StringComparison.OrdinalIgnoreCase)
            ? SharedData.Roles.Admin
            : SharedData.Roles.Consumer;

        // привязка юзера к роли
        await userManager.AddToRoleAsync(user, role);

        return true;
    }

    // public AppUser GetRegisteredUser(RegisterRequestDto registerRequestDto)
    public AppUser GetUser(IRequestDto requestDto)
    {
        return dbContext
            .AppUsers
            .FirstOrDefault(u => u.UserName.ToLower() == requestDto.UserName.ToLower());
    }

    public bool UserExistsById(string userId)
    {
        return !string.IsNullOrWhiteSpace(userId) &&
            dbContext.AppUsers.FirstOrDefault(u => u.Id.ToLower() == userId.ToLower()) != null;
    }

    #endregion
}