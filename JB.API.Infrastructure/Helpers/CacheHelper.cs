using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Infrastructure.Helpers
{
    public static class CacheHelper
    {
        public static T Get<T>(this IDistributedCache cache, string key, ILogger logger = null) where T : class
        {
            T result;

            try
            {
                byte[] bytes = cache.Get(key);
                if (bytes == null)
                    return null;

                result = DeserializeObject<T>(bytes);
            }
            catch (Exception e)
            {
                result = null;
                logger.LogError(e, e.Message);
            }

            return result;
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default, ILogger logger = null) where T : class
        {
            T result;

            try
            {
                byte[] bytes = await cache.GetAsync(key);
                if (bytes == null)
                    return null;

                result = DeserializeObject<T>(bytes);
            }
            catch (Exception e)
            {
                result = null;
                logger.LogError(e, e.Message);
            }

            return result;
        }
        public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, ILogger logger = null) where T : class
        {
            try
            {
                var bytes = SerializeObject(value);
                cache.Set(key, bytes, options);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default, ILogger logger = null) where T : class
        {
            try
            {
                var bytes = SerializeObject(value);
                await cache.SetAsync(key, bytes, options, token);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }

        public static byte[] SerializeObject<T>(T value) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
        public static T DeserializeObject<T>(byte[] bytes) => JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));

        public static T Get<T>(this IDistributedCache cache, string key, int id, ILogger logger = null) where T : class
            => Get<T>(cache, $"{key}-{id}", logger);

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, int id, CancellationToken token = default, ILogger logger = null) where T : class
            => await GetAsync<T>(cache, $"{key}-{id}", token, logger);

        public static void Set<T>(this IDistributedCache cache, string key, int id, T value, DistributedCacheEntryOptions options, ILogger logger = null) where T : class
            => Set<T>(cache, $"{key}-{id}", value, options, logger);

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, int id, T value, DistributedCacheEntryOptions options, CancellationToken token = default, ILogger logger = null) where T : class
            => await SetAsync<T>(cache, $"{key}-{id}", value, options, token, logger);

        public static async Task RemoveAsync(this IDistributedCache cache, string key, int id, CancellationToken token = default)
            => await cache.RemoveAsync($"{key}-{id}", token);

        public static void Remove(this IDistributedCache cache, string key, int id)
            => cache.Remove($"{key}-{id}");
    }
}
