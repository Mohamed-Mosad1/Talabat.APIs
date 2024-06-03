using Talabat.Core;
using Talabat.Core.Entities.Product;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(specParams);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productId);

            var product = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            return product;
        }

        public async Task<int> GetCountAsync(ProductSpecParams specParams)
        {
            var countSpec = new ProductWithFilterationForCountSpecifications(specParams);

            var count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);

            return count;
        }

        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
        {
            return await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
        }

        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
        {
            return await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
        }

    }
}
