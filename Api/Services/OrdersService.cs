using Api.Data;

namespace Api.Services
{
    public class OrdersService
    {
        private readonly AppDbContext dbContext;

        public OrdersService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}