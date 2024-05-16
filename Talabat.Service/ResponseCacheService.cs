using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Service
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _redisDatabase;

        public ResponseCacheService(IConnectionMultiplexer Redis)
        {
            _redisDatabase = Redis.GetDatabase();
        }

        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan expireTime)
        {
            if (response is null) return;

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var serializedResponse = JsonSerializer.Serialize(response, options);

            await _redisDatabase.StringSetAsync(cacheKey, serializedResponse, expireTime);
        }

        public async Task<string?> GetCachedResponse(string cacheKey)
        {
            var cacheResponse = await _redisDatabase.StringGetAsync(cacheKey);

            if (cacheResponse.IsNullOrEmpty) return null;

            return cacheResponse;
        }
    }
}
