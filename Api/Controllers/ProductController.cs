using System.Net;
using Api.Data;
using Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProductsController : StoreController
    {
        public ProductsController(IStorage storage) : base(storage)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAll()
        {
            ResponseServer response = new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = await Task.FromResult(storage.GetAllProducts())
            };

            return Ok(response);
        }
    }
}