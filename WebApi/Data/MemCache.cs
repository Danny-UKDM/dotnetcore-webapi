using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Caching;
using Microsoft.Extensions.Logging;

namespace WebApi.Data
{
    public sealed class MemCache<T> where T : class
    {
        private static readonly ILogger<MemCache<T>> Logger = new Logger<MemCache<T>>(new LoggerFactory());
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private readonly MemoryCache _memoryCache;

        public MemCache() => _memoryCache = MemoryCache.Default;

        public async Task<T> Get(string cacheKey, Func<Task<T>> retrievalFuncAsync, TimeSpan cacheTimeout)
        {
            var cached = _memoryCache.Get(cacheKey);
            if (cached != null)
            {
                Logger.LogDebug($"Retrieved CacheKey: {cacheKey}");
                return (T)cached;
            }

            await _semaphoreSlim.WaitAsync();

            try
            {
                //Double check it's not been retrieved whilst awaiting the lock
                cached = _memoryCache.Get(cacheKey);
                if (cached != null)
                {
                    Logger.LogDebug($"Retrieved CacheKey: {cacheKey}");
                    return (T)cached;
                }

                var result = await retrievalFuncAsync();

                //Don't cache but return result on failure
                if (result == null)
                    return null;

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.UtcNow.Add(cacheTimeout),
                    Priority = CacheItemPriority.Default
                };

                Logger.LogDebug($"Added CacheKey: {cacheKey}");
                _memoryCache.Set(cacheKey, result, cacheItemPolicy);

                return result;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
