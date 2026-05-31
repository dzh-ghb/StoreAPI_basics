using System.Net;
using Api.Common;
using Api.Data;
using Api.Model;
using Api.ModelDto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Payment
{
    public class FakePaymentService : IPaymentService
    {
        private readonly AppDbContext dbContext;

        public FakePaymentService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ActionResult<ServerResponse>> HandlePaymentAsync(string userId, int orderHeaderId, string cardNumber)
        {
            var shoppingCart = await new ShoppingCartService(dbContext).GetShoppingCartAsync(userId); // TODO: внедрить зависимость через конструктор

            if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count == 0)
            {
                return new BadRequestObjectResult(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Корзина пуста или не найдена" }
                });
            }

            var orderHeader = await new OrdersService(dbContext).GetOrderByIdAsync(orderHeaderId); // TODO: внедрить зависимость через конструктор

            if (orderHeader == null)
            {
                return new BadRequestObjectResult(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Такого заказа не существует" }
                });
            }

            await Task.Delay(5000);

            var paymentResponse = new PaymentResponse
            {
                Success = cardNumber == "1111 3333 3333 7777"
            };

            if (paymentResponse.Success)
            {
                paymentResponse.IntentId = "test_success_intent_id";
                paymentResponse.Secret = "test_success_secret";

                await new OrdersService(dbContext) // TODO: внедрить зависимость через конструктор
                    .UpdateOrderHeaderAsync(
                        orderHeaderId,
                        new OrderHeaderUpdateDto { Status = SharedData.OrderStatuses.ReadyToShip }
                    );
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

            return new OkObjectResult(new ServerResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = orderHeader
            });
        }
    }
}