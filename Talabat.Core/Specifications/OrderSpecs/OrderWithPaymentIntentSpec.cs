using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Orders_Aggregate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderWithPaymentIntentSpec : BaseSpecifications<Order>
    {
        public OrderWithPaymentIntentSpec(string paymentIntentId) :base(order=>order.PaymentInstantId == paymentIntentId)
        {
            
        }
    }
}
