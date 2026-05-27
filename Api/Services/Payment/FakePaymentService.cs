using Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Payment
{
    public class FakePaymentService : IPaymentService
    {
        public Task<ActionResult<ServerResponse>> HandlePaymentAsync(string userId, int orderHeaderId, string cardNumber)
        {
            throw new NotImplementedException();
        }
    }
}