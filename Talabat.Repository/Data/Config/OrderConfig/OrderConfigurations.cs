using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Config.OrderConfig
{
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(order => order.ShippingAddress, shippingAddress => shippingAddress.WithOwner());
            builder.Property(order => order.Status).HasConversion(orderStatus => orderStatus.ToString(), orderStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus));
            builder.HasOne(order => order.DeliveryMethod).WithMany().OnDelete(DeleteBehavior.SetNull);
            //builder.HasIndex("DeliveryMethodId").IsUnique(true);
            builder.Property(order => order.SubTotal).HasColumnType("decimal(12, 2)");
            builder.HasMany(order => order.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

        }
    }
}
