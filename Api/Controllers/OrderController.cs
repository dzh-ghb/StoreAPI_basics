using Api.Services;

namespace Api.Controllers
{
    public class OrderController : StoreController
    {
        private readonly OrdersService ordersService;

        public OrderController(
            IStorage storage,
            OrdersService ordersService)
            : base(storage)
        {
            this.ordersService = ordersService;
        }
    }
}