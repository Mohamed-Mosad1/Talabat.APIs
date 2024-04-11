using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepository;

        public ProductController(IGenericRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;
        }
    }
}
