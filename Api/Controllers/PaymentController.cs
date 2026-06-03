using System.Net;
using Api.Model;
using Api.Services.Payment;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class PaymentController : StoreController
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IStorage storage, IPaymentService paymentService) : base(storage)
        {
            this.paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<ServerResponse>> MakePayment(string userId, int orderHeaderId, string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(userId) && orderHeaderId <= 0 && string.IsNullOrWhiteSpace(cardNumber))
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Введите данные" }
                });
            }

            var userExistsById = storage.UserExistsById(userId);

            if (string.IsNullOrWhiteSpace(userId) || !userExistsById)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Пользователя с таким ID не существует" }
                });
            }

            try
            {
                return await paymentService.HandlePaymentAsync(userId, orderHeaderId, cardNumber);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessages = { "Ошибка", ex.Message }
                });
            }
        }
    }
}