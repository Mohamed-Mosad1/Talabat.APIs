using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entities.Product;

namespace Talabat.Repository.Data.Config.ProductConfig
{
	internal class ProductBrandConfigurations : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(EntityTypeBuilder<ProductBrand> builder)
        {
            builder.Property(B => B.Name).IsRequired();
            builder.HasIndex(B => B.Name).IsUnique();
        }
    }
}
