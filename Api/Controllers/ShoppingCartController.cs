using System.Net;
using Api.Model;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ShoppingCartController : StoreController
    {
        private readonly ShoppingCartService shoppingCartService;

        public ShoppingCartController(
            IStorage storage,
            ShoppingCartService shoppingCartService
        ) : base(storage)
        {
            this.shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<ActionResult<ServerResponse>> AppendOrUpdateItemInCart(string userId, int productId, int updateQuantity)
        {
            bool isUserExist = await Task.FromResult(storage.IsUserFindedById(userId));
            if (isUserExist == false)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Пользователя с таким ID не существует" }
                });
            }

            if (productId <= 0)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Указан некорректный ID товара" }
                });
            }

            Product product = await Task.FromResult(storage.GetProduct(productId));
            if (product == null)
            {
                return NotFound(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = { "Товар по указанному ID не найден" }
                });
            }

            ShoppingCart shoppingCart = await shoppingCartService.GetShoppingCartAsync(userId);
            if (shoppingCart == null && updateQuantity > 0)
            {
                await shoppingCartService.CreateNewCartAsync(userId, productId, updateQuantity);
            }
            else if (shoppingCart != null)
            {
                await shoppingCartService.UpdateExistingCartAsync(shoppingCart, productId, updateQuantity);
            }

            return Ok(new ServerResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = shoppingCart
            });
        }

        [HttpGet]
        public async Task<ActionResult<ServerResponse>> GetShoppingCart(string userId)
        {
            bool isUserExist = await Task.FromResult(storage.IsUserFindedById(userId));
            if (isUserExist == false)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Пользователя с таким ID не существует" }
                });
            }

            ShoppingCart shoppingCart = await shoppingCartService.GetShoppingCartAsync(userId);
            if (shoppingCart == null)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Ошибка получения корзины" }
                });
            }

            return Ok(new ServerResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = shoppingCart
            });
        }
    }
}