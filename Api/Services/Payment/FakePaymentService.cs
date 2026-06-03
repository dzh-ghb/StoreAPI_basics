using System.Net;
using Api.Common;
using Api.Model;
using Api.ModelDto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Payment
{
    public class FakePaymentService : IPaymentService
    {
        // private readonly AppDbContext dbContext;
        private readonly ShoppingCartService cartService;
        private readonly OrdersService ordersService;

        public FakePaymentService(/*AppDbContext dbContext,*/ ShoppingCartService cartService, OrdersService ordersService)
        {
            // this.dbContext = dbContext;
            this.cartService = cartService;
            this.ordersService = ordersService;
        }

        public async Task<ActionResult<ServerResponse>> HandlePaymentAsync(string userId, int orderHeaderId, string cardNumber)
        {
            var shoppingCart = await cartService.GetShoppingCartAsync(userId); // TODO: внедрить зависимость через конструктор
            // var shoppingCart = await new ShoppingCartService(dbContext).GetShoppingCartAsync(userId); // TODO: внедрить зависимость через конструктор
            // var shoppingCart = await dbContext
            //     .ShoppingCarts
            //     .Include(u => u.CartItems)
            //     .ThenInclude(u => u.Product)
            //     .FirstOrDefaultAsync(u => u.UserId == userId);

            if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count == 0)
            {
                return new BadRequestObjectResult(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Корзина пуста или не найдена" }
                });
            }

            // shoppingCart.TotalAmount = shoppingCart
            //     .CartItems
            //     .Sum(i => i.Quantity * i.Product.Price);

            var orderHeader = await ordersService.GetOrderByIdAsync(orderHeaderId); // TODO: внедрить зависимость через конструктор
            // var orderHeader = await new OrdersService(dbContext).GetOrderByIdAsync(orderHeaderId); // TODO: внедрить зависимость через конструктор
            // var orderHeader = await dbContext
            //     .OrderHeaders
            //     .FindAsync(orderHeaderId);

            if (orderHeader == null || orderHeader.AppUserId != userId)
            {
                return new BadRequestObjectResult(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Такого заказа не существует" }
                });
            }

            orderHeader.OrderTotalAmount = shoppingCart.TotalAmount;
            orderHeader.TotalCount = shoppingCart.CartItems.Count;

            await Task.Delay(5000);

            var paymentResponse = new PaymentResponse
            {
                Success = cardNumber == "1111 3333 3333 7777"
            };

            if (paymentResponse.Success)
            {
                paymentResponse.IntentId = "test_success_intent_id";
                paymentResponse.Secret = "test_success_secret";

                await ordersService // TODO: внедрить зависимость через конструктор
                // await new OrdersService(dbContext) // TODO: внедрить зависимость через конструктор
                    .UpdateOrderHeaderAsync(
                        orderHeaderId,
                        new OrderHeaderUpdateDto
                        {
                            OrderHeaderId = orderHeaderId,
                            Status = SharedData.OrderStatuses.ReadyToShip
                        }
                    );
                // orderHeader.Status = SharedData.OrderStatuses.ReadyToShip;
                // await dbContext.SaveChangesAsync();
            }
            else
            {
                paymentResponse.ErrorMessage = "Ошибка платежа";

                return new BadRequestObjectResult(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { paymentResponse.ErrorMessage }
                });
            }

            orderHeader.User = null; // заглушка: считаем, что детальная инфа о юзере не нужна

            return new OkObjectResult(new ServerResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = orderHeader
            });
        }
    }
}