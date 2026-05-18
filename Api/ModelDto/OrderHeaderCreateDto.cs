using System.ComponentModel.DataAnnotations;

namespace Api.ModelDto
{
    // модель для передачи данных при создании заголовка заказа (весь заказ)
    public class OrderHeaderCreateDto
    {
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string AppUserId { get; set; }
        public double OrderTotalAmount { get; set; }
        public string Status { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<OrderDetailsCreateDto> OrderDetailDto { get; set; }
    }
}