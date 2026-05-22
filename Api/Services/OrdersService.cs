using Api.Common;
using Api.Data;
using Api.Model;
using Api.ModelDto;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class OrdersService
    {
        private readonly AppDbContext dbContext;

        public OrdersService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<OrderHeader> CreateOrderAsync(
            OrderHeaderCreateDto orderHeaderCreateDto
        )
        {
            var order = new OrderHeader
            {
                CustomerName = orderHeaderCreateDto.CustomerName,
                CustomerPhoneNumber = orderHeaderCreateDto.CustomerPhoneNumber,
                CustomerEmail = orderHeaderCreateDto.CustomerEmail,
                AppUserId = orderHeaderCreateDto.AppUserId,
                OrderTotalAmount = orderHeaderCreateDto.OrderTotalAmount,
                OrderDateTime = DateTime.UtcNow,
                Status = string.IsNullOrEmpty(orderHeaderCreateDto.Status)
                    ? SharedData.OrderStatuses.Pending
                    : orderHeaderCreateDto.Status,
                TotalCount = orderHeaderCreateDto.TotalCount,
            };

            await dbContext.OrderHeaders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            foreach (var orderDetailsDto in orderHeaderCreateDto.OrderDetailsDto)
            {
                var orderDetails = new OrderDetails
                {
                    OrderHeaderId = order.OrderHeaderId,
                    ProductId = orderDetailsDto.ProductId,
                    Quantity = orderDetailsDto.Quantity,
                    ItemName = orderDetailsDto.ItemName,
                    Price = orderDetailsDto.Price,
                };

                await dbContext.OrderDetails.AddAsync(orderDetails);
            }

            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<OrderHeader> GetOrderById(int id)
        {
            return await dbContext
                .OrderHeaders
                .Include(i => i.OrderDetailItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(u => u.OrderHeaderId == id);
        }
    }
}