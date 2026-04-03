using Api.Data;
using Api.Model;
using Api.ModelDto;

public class PostgreSqlEfStorage : IStorage
{
    protected readonly AppDbContext dbContext;

    public PostgreSqlEfStorage(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Product AddProduct(ProductCreateDto productCreateDto)
    {
        Product item = new()
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            SpecialTag = productCreateDto.SpecialTag,
            Category = productCreateDto.Category,
            Price = productCreateDto.Price,
            Image = productCreateDto.Image
        };

        // добавление в БД
        dbContext.Products.Add(item);
        dbContext.SaveChanges();

        return item;
    }

    public List<Product> GetAllProducts()
    {
        return dbContext.Products.ToList(); // БД.таблица.преобразовать_в_список()
    }

    public Product GetProduct(int id)
    {
        return dbContext.Products.FirstOrDefault(x => x.Id == id);
    }
}