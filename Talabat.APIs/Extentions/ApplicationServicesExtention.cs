using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Helpers;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.APIs.Error;

namespace Talabat.APIs.Extentions
{
    public static class ApplicationServicesExtention
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services)
        {
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
