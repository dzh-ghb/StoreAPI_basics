using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Model
{
    // модель товара в корзине
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ShoppingCartId { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}