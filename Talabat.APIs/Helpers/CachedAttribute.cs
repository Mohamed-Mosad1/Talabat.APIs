using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expireTimeInSeconds;

        public CachedAttribute(int expireTimeInSeconds)
        {
            _expireTimeInSeconds = expireTimeInSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cacheResponse = await cacheService.GetCachedResponse(cacheKey);

            if(!string.IsNullOrEmpty(cacheResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cacheResponse,
                    ContentType = "Application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;
                return;
            }

            var excuteEndpointContext = await next.Invoke(); // Excute Endpoint
        
            if(excuteEndpointContext.Result is OkObjectResult result)
            {
               await cacheService.SetCacheResponseAsync(cacheKey, result.Value, TimeSpan.FromSeconds(_expireTimeInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path); // api/Products

            foreach (var (Key, Value) in request.Query.OrderBy(x => x.Key))
            {
                // sort = Name
                // PageIndex = 1
                // PageSize = 5

                // api/Products|Sort-Name|PageIndex-1|PageSize-5
                keyBuilder.Append($"|{Key}-{Value}");
            }

            return keyBuilder.ToString();
        }
    }
}
