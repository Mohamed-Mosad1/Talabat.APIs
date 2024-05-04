using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entities.Orders_Aggregate;

namespace Talabat.Repository.Data.Config.OrderConfig
{
    internal class OrderItemsConfigurations : IEntityTypeConfiguration<OrderItems>
    {
        public void Configure(EntityTypeBuilder<OrderItems> builder)
        {
            builder.OwnsOne(orderItem => orderItem.Product, product => product.WithOwner());
            builder.Property(orderItem => orderItem.Price).HasColumnType("decimal(12, 2)");
        }
    }
}
