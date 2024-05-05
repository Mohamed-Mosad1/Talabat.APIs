using Talabat.Core.Entities.Product;
using Talabat.Core.Specifications.ProductSpecs;

namespace Talabat.Core
{
    public interface IProductService
    {
        Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams specParams);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<int> GetCountAsync(ProductSpecParams specParams);
        Task<IReadOnlyList<ProductBrand>> GetBrandsAsync();
        Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync();

    }
}
