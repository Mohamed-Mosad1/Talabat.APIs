using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Helpers;
using Talabat.Repository;
using Talabat.APIs.Error;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Basket;
using Talabat.Core;
using Talabat.Core.Services.Contract;
using Talabat.Service.OrderService;
using Talabat.Service;

namespace Talabat.APIs.Extentions
{
	public static class ApplicationServicesExtention
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

			services.AddScoped(typeof(IOrderService), typeof(OrderService));

			services.AddScoped(typeof(IProductService), typeof(ProductService));

			services.AddScoped<IBasketRepository, BasketRepository>();

			//services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			services.AddAutoMapper(typeof(MappingProfile));

			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Values.Where(V => V.Errors.Count() > 0)
														 .SelectMany(V => V.Errors)
														 .Select(E => E.ErrorMessage)
														 .ToList();

					var response = new ApiValidationErrorResponse()
					{
						Errors = errors
					};

					return new BadRequestObjectResult(response);
				};
			});

			return services;
		}
	}
}
