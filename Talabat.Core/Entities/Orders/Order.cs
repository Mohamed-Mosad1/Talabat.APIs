using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Orders
{
    public class Order : BaseEntity
    {
        public Order()
        {
            
        }

        public Order(string buyerEmail, OrderStatus status, OrderAddress shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItems> orderItems, decimal subTotal, decimal total)
        {
            BuyerEmail = buyerEmail;
            Status = status;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            SubTotal = subTotal;
            Total = total;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; }
        public OrderAddress ShippingAddress { get; set; }

        //public int DeliveryMethodId { get; set; } // FK
        public DeliveryMethod DeliveryMethod { get; set; } // One

        public ICollection<OrderItems> OrderItems { get; set; } = new HashSet<OrderItems>(); // Many
        public decimal SubTotal { get; set; }

        [NotMapped]
        public decimal Total { get; set; }

        public decimal GetTotal() => SubTotal + DeliveryMethod.Cost;

        public string PaymentInstantId { get; set; }
    }
}
