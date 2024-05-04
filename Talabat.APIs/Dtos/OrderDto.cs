using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Orders_Aggregate;

namespace Talabat.APIs.Dtos
{
    public class OrderDto
    {
        [Required]
        public string BuyerEmail { get; set; }
        [Required]
        public string BasketId { get; set; }
        [Required]
        public int DeliveryMethodId { get; set; }
        public OrderAddressDto ShippingAddress { get; set; }
    }
}
