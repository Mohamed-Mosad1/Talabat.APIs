using Talabat.Core;
using Talabat.Core.Entities.Orders_Aggregate;
using Talabat.Core.Entities.Product;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.OrderSpecs;

namespace Talabat.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork
            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, OrderAddress shippingAddress)
        {
            var basket = await _basketRepository.GetBasketByIdAsync(basketId);
            var orderItems = new List<OrderItems>();

            if (basket?.Items?.Count > 0)
            {
                var productRepo = _unitOfWork.Repository<Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepo.GetByIdAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItems(productItemOrdered, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
            }

            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal);

            _unitOfWork.Repository<Order>().AddAsync(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;

            return order;
        }


        public async Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();

            var spec = new OrderSpecifications(buyerEmail);

            var orders = await orderRepo.GetAllWithSpecAsync(spec);

            return orders;
        }


        public Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo = _unitOfWork.Repository<Order>();

            var orderSpec = new OrderSpecifications(orderId, buyerEmail);

            var order = orderRepo.GetWithSpecAsync(orderSpec);

            return order;
        }


        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        }

    }
}
