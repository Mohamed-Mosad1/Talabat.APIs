using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepository;

        public ProductsController(IGenericRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;
        }

        // /api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productsRepository.GetAllAsync();

            return Ok(products); // 200
        }

        // /api/Products
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProducts(int id)
        {
            var products = await _productsRepository.GetByIdAsync(id);

            if (products is null)
               return NotFound(new {Message = "Not Found", StatusCode = 404}); // 404

            return Ok(products); // 200
            
        }


    }
}
