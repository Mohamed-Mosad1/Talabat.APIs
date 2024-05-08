using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Product;

namespace AdminDashboard.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; } 

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public string PictureUrl { get; set; } 

        [Required(ErrorMessage = "Price is Required")]
        [Range(1, 100000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "ProductBrandId is Required")]
		public int BrandId { get; set; } // FK
		public ProductBrand Brand { get; set; }

        [Required(ErrorMessage = "ProductCategoryId is Required")]
		public int CategoryId { get; set; } // FK
		public ProductCategory Category { get; set; } 
	}
}
