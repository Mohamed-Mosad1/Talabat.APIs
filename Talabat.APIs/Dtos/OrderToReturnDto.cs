using Talabat.Core.Entities.Orders_Aggregate;

namespace Talabat.APIs.Dtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } 
        public string Status { get; set; }
        public OrderAddress ShippingAddress { get; set; }
        public string DeliveryMethod { get; set; } 
        public decimal DeliveryMethodCoast { get; set; } 
        public ICollection<OrderItemsDto> OrderItems { get; set; } = new HashSet<OrderItemsDto>(); // [Many]
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string PaymentInstantId { get; set; } = string.Empty;
    }
}
