using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        // This is your Stripe CLI webhook secret for testing your endpoint locally.
        const string endpointSecret = "whsec_656cd9c13c9e35d5fc593b656613e1859b12c9a9befc689ba486bfa2c6976270";

        public PaymentsController(
            IPaymentService paymentService,
            IMapper mapper
            )
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var customerBasket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (customerBasket == null)
            {
                return BadRequest(new ApiResponse(400, "There is a problem with your Basket"));
            }

            var mappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(customerBasket);

            return Ok(mappedBasket);
        }


        [HttpPost("webhook")] // POST: /api/Payments/webhook
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                   await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                   await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, true);
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(new ApiResponse(400));
            }
        }
    }



}
