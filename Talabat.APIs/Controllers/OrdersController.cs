using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.Core.Entities.Orders_Aggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(
            IOrderService orderService,
            IMapper mapper
            )
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [ProducesResponseType( typeof( Order ), StatusCodes.Status200OK )]
        [ProducesResponseType( typeof( ApiResponse ), StatusCodes.Status400BadRequest )]
        [HttpPost] // POST: /api/Orders
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var shippingAddress = _mapper.Map<OrderAddressDto, OrderAddress>(orderDto.ShippingAddress);

            var order = await _orderService.CreateOrderAsync(orderDto.BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, shippingAddress);

            if (order == null) return BadRequest(new ApiResponse(400));

            return Ok(order);
        }

        [HttpGet] // GET: /api/Orders
        public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser(string buyerEmail)
        {
            var orders = await _orderService.GetOrderForUserAsync(buyerEmail);

            return Ok(orders);
        }


        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK )]
        [ProducesResponseType(typeof(Order), StatusCodes.Status404NotFound )]
        [HttpGet("{id}")] // GET: /api/Orders/{id}?buyerEmail=mohamed@gmail.com
        public async Task<ActionResult<Order>> GetOrderByIdForUser(int id, string buyerEmail)
        {
            var orders = await _orderService.GetOrderByIdForUserAsync(id, buyerEmail);

            if (orders is null) return NotFound(new ApiResponse(404));

            return Ok(orders);
        }




    }
}
