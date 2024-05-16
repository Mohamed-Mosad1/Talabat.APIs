using Microsoft.Extensions.Configuration;
using Stripe;
using Talabat.Core;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Orders_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;
using Product = Talabat.Core.Entities.Product.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IConfiguration configuration,
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork
            )
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            // Get Secret key
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            // Get Basket
            var basket = await _basketRepository.GetBasketByIdAsync(basketId);
            if (basket is null) return null;

            var shippingPrice = 0M;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);

                shippingPrice = deliveryMethod.Cost;
            }

            if (basket.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if (item.Price != product.Price)
                    {
                        item.Price = product.Price;
                    }
                }
            }

            var subTotal = basket.Items.Sum(item => item.Price * item.Quantity);

            // Create Payment Intent
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId)) // Create
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(subTotal * 100 + shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(subTotal * 100 + shippingPrice * 100)
                };

                paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string paymentIntentId, bool flag)
        {
            var spec = new OrderWithPaymentIntentSpec(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (flag)
            {
                order.Status = OrderStatus.PaymentSucceeded;
            }
            else
            {
                order.Status = ~OrderStatus.PaymentFailed;
            }

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            return order;
        }
    }
}
