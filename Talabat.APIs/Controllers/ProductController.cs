using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpecs;

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
            var spec = new ProductWithBrandAndCategorySpecifications();

            var products = await _productsRepository.GetAllWithSpecAsync(spec);
            
            return Ok(products); // 200
        }

        // /api/Product
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);

            var product = await _productsRepository.GetWithSpecAsync(spec);

            if (product is null)
               return NotFound(new {Message = "Not Found", StatusCode = 404}); // 404

            return Ok(product); // 200
            
        }


    }
}
