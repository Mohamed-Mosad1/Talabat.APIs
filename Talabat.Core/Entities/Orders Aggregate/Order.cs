using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Orders_Aggregate
{
    public class Order : BaseEntity
    {
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public OrderAddress ShippingAddress { get; set; }
        //public int DeliveryMethodId { get; set; } // FK
        public DeliveryMethod? DeliveryMethod { get; set; } // Navigational Property [ONE]
        public ICollection<OrderItems> OrderItems { get; set; } = new HashSet<OrderItems>(); // [Many]
        public decimal SubTotal { get; set; }

        //[NotMapped]
        //public decimal Total => SubTotal + DeliveryMethod.Cost;
        public decimal GetTotal() => SubTotal + DeliveryMethod.Cost;
        public string PaymentInstantId { get; set; } = string.Empty;


        private Order() { }

        public Order(string buyerEmail, OrderAddress shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItems> orderItems, decimal subTotal)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            SubTotal = subTotal;
        }

    }
}
