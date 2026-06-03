using Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Payment
{
    public interface IPaymentService
    {
        // метод обработки платежа
        Task<ActionResult<ServerResponse>> HandlePaymentAsync(string userId, int orderHeaderId, string cardNumber);
    }
}