using Api.Model;
using Api.ModelDto;
using Microsoft.AspNetCore.Identity;

public interface IStorage
{
    #region ProductsInfrastructure
    Task<Product> AddProduct(ProductCreateDto productCreateDto);

    List<Product> GetAllProducts();

    Task<List<Product>> GetProductsWithPagination(int skip = 0, int take = 5);

    Product GetProduct(int id);

    Task<Product> UpdateProduct(int id, ProductUpdateDto productUpdateDto);

    Task<bool> RemoveProductAsync(int id);

    int GetProductsCount();

    #endregion

    #region AuthInfrastructure
    Task<bool> AddUser(RegisterRequestDto registerRequestDto, UserManager<AppUser> userManager/*, RoleManager<IdentityRole> roleManager*/);

    AppUser GetUser(IRequestDto requestDto);

    // AppUser GetLoginnedUser(LoginRequestDto loginRequestDto);

    bool UserExistsById(string userId);

    #endregion
}