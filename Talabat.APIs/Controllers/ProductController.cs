using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepository;
        private readonly IGenericRepository<ProductBrand> _productBrandsRepository;
        private readonly IGenericRepository<ProductCategory> _productCategoriesRepository;
        private readonly IMapper _mapper;

        public ProductsController(
            IGenericRepository<Product> productsRepository,
            IGenericRepository<ProductBrand> productBrandsRepository,
            IGenericRepository<ProductCategory> productCategoriesRepository,
            IMapper mapper)
        {
            _productsRepository = productsRepository;
            _productBrandsRepository = productBrandsRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _mapper = mapper;
        }

        // /api/Products
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(specParams);

            var products = await _productsRepository.GetAllWithSpecAsync(spec);

            var result = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(result); // 200
        }

        // /api/Products/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductToReturnDto>>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(id);

            var product = await _productsRepository.GetWithSpecAsync(spec);

            if (product is null)
                return NotFound(new ApiResponse(404)); // 404

            var result = _mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(result); // 200
        }

        // GET /api/Products/brands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _productBrandsRepository.GetAllAsync();

            return Ok(brands);
        }

        // GET /api/Products/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetProductCategories()
        {
            var categories = await _productCategoriesRepository.GetAllAsync();

            return Ok(categories);
        }




    }
}
