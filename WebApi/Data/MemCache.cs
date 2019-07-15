using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Caching;
using Microsoft.Extensions.Logging;

namespace WebApi.Data
{
    public sealed class MemCache<T> where T : class
    {
        private readonly ILogger<MemCache<T>> _logger;
        private readonly MemoryCache _memoryCache;
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

        public MemCache(ILogger<MemCache<T>> logger)
        {
            _logger = logger;
            _memoryCache = MemoryCache.Default;
        }

        public async Task<T> Get(string cacheKey, Func<Task<T>> retrievalFuncAsync, TimeSpan cacheTimeout)
        {
            var cached = this._memoryCache.Get(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug($"Retrieved CacheKey: {cacheKey}");
                return (T)cached;
            }

            await SemaphoreSlim.WaitAsync();

            try
            {
                //Double check it's not been retrieved whilst awaiting the lock
                cached = this._memoryCache.Get(cacheKey);
                if (cached != null)
                {
                    _logger.LogDebug($"Retrieved CacheKey: {cacheKey}");
                    return (T)cached;
                }

                var result = await retrievalFuncAsync();

                //Don't cache but return result on failure
                if (result == null)
                {
                    return null;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.UtcNow.Add(cacheTimeout),
                    Priority = CacheItemPriority.Default
                };

                _logger.LogDebug($"Added CacheKey: {cacheKey}");
                _memoryCache.Set(cacheKey, result, cacheItemPolicy);

                return result;
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}
