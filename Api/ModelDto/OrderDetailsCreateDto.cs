using System.ComponentModel.DataAnnotations;

namespace Api.ModelDto
{
    // модель для передачи данных при создании деталей заказа (продукт в заказе)
    public class OrderDetailsCreateDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        public double Price { get; set; }
    }
}