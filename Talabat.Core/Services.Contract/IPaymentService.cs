using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Basket;

namespace Talabat.Core.Services.Contract
{
    public interface IPaymentService
    {
        // Method To Create or Update Payment Intent

        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
    }
}
