using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Helpers;
using Talabat.Repository;
using Talabat.APIs.Error;
using Talabat.Core.Repositories.Contract;
using Microsoft.AspNetCore.Builder;
using Talabat.Core.Services.Contract;
using Talabat.Repository.Basket;
using Talabat.Service.AuthService;

namespace Talabat.APIs.Extentions
{
    public static class ApplicationServicesExtention
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBasketRepository, BasketRepository>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddAutoMapper(typeof(MappingProfile));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();

                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                });
            });

            return services;
        }
    }
}
