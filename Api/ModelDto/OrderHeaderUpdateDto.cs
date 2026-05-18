using System.ComponentModel.DataAnnotations;

namespace Api.ModelDto
{
    // модель для передачи данных при обновлении заголовка заказа (весь заказ)
    public class OrderHeaderUpdateDto
    {
        [Required]
        public int OrderHeaderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
    }
}