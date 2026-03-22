using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProductController : StoreController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await Task.FromResult<string>("_DZHITS"));
        }
    }
}