using System.Net;
using Api.Model;
using Api.ModelDto;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<ActionResult<ServerResponse>> Create(
            [FromBody] OrderHeaderCreateDto orderHeaderCreateDto
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Невалидное состояние модели заказа" }
                });
            }

            try
            {
                var order = await ordersService.CreateOrderAsync(orderHeaderCreateDto);
                // order.OrderDetailItems = null;

                return Ok(new ServerResponse
                {
                    StatusCode = HttpStatusCode.Created,
                    Result = order
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Ошибка:", ex.Message }
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServerResponse>> GetOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Указан некорректный ID" }
                });
            }

            try
            {
                var orderHeader = await ordersService.GetOrderById(id);

                if (orderHeader == null)
                {
                    return NotFound(new ServerResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessages = { "Заказ не найден" }
                    });
                }

                return Ok(new ServerResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = orderHeader
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Ошибка:", ex.Message }
                });
            }
        }
    }
}