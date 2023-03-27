using System;
using System.Threading.Tasks;

namespace AccountManager.Application.Caching
{
    public static class CachingExtensions
    {
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }

        public static async Task<T> GetAsync<T>(this ICacheManager cacheManager, string key, Func<Task<T>> acquire)
        {
            return await GetAsync(cacheManager, key, 60, acquire);
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            try
            {
                if (cacheManager.IsSet(key)) return cacheManager.Get<T>(key);

                var result = acquire();
                cacheManager.Set(key, result, cacheTime);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<T> GetAsync<T>(this ICacheManager cacheManager, string key, int cacheTime,
            Func<Task<T>> acquire)
        {
            if (cacheManager.IsSet(key)) return await Task.Run(() => cacheManager.Get<T>(key));

            var result = await acquire();
            cacheManager.Set(key, result, cacheTime);
            return result;
        }
    }
}