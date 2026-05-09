using Api.Data;
using Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ShoppingCartService
    {
        private readonly AppDbContext dbContext;

        public ShoppingCartService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateNewCartAsync(string userId, int productId, int quantity)
        {
            ShoppingCart newCart = new ShoppingCart
            {
                UserId = userId
                // CartItems = new List<CartItem>()
            };

            await dbContext.ShoppingCarts.AddAsync(newCart);
            await dbContext.SaveChangesAsync();

            CartItem newCartItem = new CartItem
            {
                ProductId = productId,
                ShoppingCartId = newCart.Id,
                Quantity = quantity,
                Product = null // Product будет взят по ProductId (ForeignKey)
            };

            await dbContext.CartItems.AddAsync(newCartItem);
            await dbContext.SaveChangesAsync();
        }

        // общий метод для обновления (изменения количества), удаления товаров в корзине
        public async Task UpdateExistingCartAsync(ShoppingCart shoppingCart, int productId, int quantityDelta)
        {
            CartItem cartItemInCart = shoppingCart
                .CartItems
                .FirstOrDefault(e => e.ProductId == productId);

            // если такого товара в корзине нет и указанное количество > 0
            if (cartItemInCart == null && quantityDelta > 0)
            {
                CartItem cartItem = new CartItem
                {
                    ProductId = productId,
                    ShoppingCartId = shoppingCart.Id,
                    Quantity = quantityDelta,
                    Product = null
                };

                await dbContext.CartItems.AddAsync(cartItem);
            }
            // если такой товар в корзине уже есть
            else if (cartItemInCart != null)
            {
                int updatedQuantity = cartItemInCart.Quantity + quantityDelta;

                /* TODO: если quantityDelta == 0, то считаем, что товар нужно удалить,
                НО для updatedQuantity значение считается как дельта, а не абсолютное (новое) значение в корзине;
                Покумекать позже - корректно ли*/
                if (quantityDelta == 0 || updatedQuantity <= 0)
                {
                    dbContext.CartItems.Remove(cartItemInCart);

                    // если товар в корзине единственный (необходимо удалить корзину)
                    if (shoppingCart.CartItems.Count == 1) // 1, т.к. изменений корзины во время работы метода не происходит
                    {
                        dbContext.ShoppingCarts.Remove(shoppingCart);
                    }
                }
                else
                {
                    cartItemInCart.Quantity = updatedQuantity;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        // метод получения корзины со всем содержимым
        public async Task<ShoppingCart> GetShoppingCartAsync(string userId)
        {
            // TODO: корректно ли?
            if (string.IsNullOrEmpty(userId))
            {
                // создание корзины, если юзер новый
                // return new ShoppingCart();
                return null;
            }

            ShoppingCart shoppingCart = await dbContext
                .ShoppingCarts
                .Include(u => u.CartItems) // ShoppingCarts >> CartItems
                .ThenInclude(u => u.Product) // CartItems >> Product
                .FirstOrDefaultAsync(u => u.UserId == userId);

            // корзина существует и не пустая (стоимость корзины)
            if (shoppingCart != null && shoppingCart.CartItems != null)
            {
                shoppingCart.TotalAmount = shoppingCart
                    .CartItems
                    .Sum(i => i.Quantity * i.Product.Price);
            }

            return shoppingCart;
        }
    }
}