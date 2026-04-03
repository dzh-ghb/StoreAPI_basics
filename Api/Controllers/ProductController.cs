using System.Net;
using Api.Data;
using Api.Model;
using Api.ModelDto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProductsController : StoreController
    {
        public ProductsController(IStorage storage) : base(storage)
        {
        }

        [HttpPost]
        public async Task<ActionResult<ServerResponse>> Create(
            [FromBody] ProductCreateDto productCreateDto
        )
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // проверка на обязательное наличие изображения
                    if (productCreateDto.Image == null
                        || productCreateDto.Image.Length == 0)
                    {
                        return BadRequest(new ServerResponse
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            IsSuccess = false,
                            ErrorMessages = { "Наличие изображения (Image) обязательно" }
                        });
                    }
                    else
                    {
                        // демо-вариант с фейковым значением
                        productCreateDto.Image = $"https://placehold.co/100";
                        // Image = productCreateDto.Image // более корректный вариант для финальной версии

                        Product item = await Task.FromResult(storage.AddProduct(productCreateDto));

                        ServerResponse response = new()
                        {
                            StatusCode = HttpStatusCode.Created,
                            Result = item
                        };
                        // возврат добавленного значения (по сути - выполнение метода GetById(id))
                        return CreatedAtRoute(nameof(GetById), new { id = item.Id }, response);
                    }
                }
                else // невалидная модель
                {
                    return BadRequest(new ServerResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessages = { "Некорректная модель данных" }
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ServerResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Возникла ошибка/исключение", ex.Message }
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ServerResponse>> GetAll()
        {
            ServerResponse response = new ServerResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = await Task.FromResult(storage.GetAllProducts())
            };

            return Ok(response);
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        public async Task<ActionResult<ServerResponse>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ServerResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = { "Указан некорректный ID" }
                });
            }

            Product result = await Task.FromResult(storage.GetProduct(id));

            if (result == null)
            {
                return NotFound(new ServerResponse
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    ErrorMessages = { "Продукт по указанному ID не найден" }
                });
            }
            return Ok(new ServerResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = result
            });
        }
    }
}