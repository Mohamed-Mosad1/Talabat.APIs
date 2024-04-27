using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Product
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }


        public int BrandId { get; set; } // FK
        public ProductBrand Brand { get; set; } = null!;

        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; } = null!;


    }
}
