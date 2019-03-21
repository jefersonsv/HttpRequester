using Foundatio.Caching;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HttpRequester
{
    public class CacheProvider
    {
        readonly TimeSpan duration;
        readonly ICacheClient cache;

        public CacheProvider(DataFoundation.Redis.RedisConnection redis, TimeSpan? duration = null)
        {
            this.duration = duration ?? TimeSpan.FromDays(30);
            cache = new RedisCacheClient(o => o.ConnectionMultiplexer(redis.Client));
        }

        public async Task<ResponseContext> GetCacheResponseContextAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var key = KeyHelper.GetKey(nameof(ResponseContext), url, postData);

            var cached = await cache.GetAsync<ResponseContext>(key);
            return cached.Value;
        }

        public async Task<ResponseContext> GetCacheResponseContextAsync(string url, string postData)
        {
            var key = KeyHelper.GetKey(nameof(ResponseContext), url, postData);

            var cached = await cache.GetAsync<ResponseContext>(key);
            return cached.Value;
        }

        public async Task<ResponseContext> GetCacheResponseContextAsync(string url)
        {
            var key = KeyHelper.GetKey(nameof(ResponseContext), url);

            var cached = await cache.GetAsync<ResponseContext>(key);
            if (cached.HasValue)
                cached.Value.HasUsedCache = true;

            return cached.Value;
        }

        async Task<ResponseContext> CachedAsync(string url, Func<Task<ResponseContext>> funcAsync, EnumCacheStrategy strategy, ResponseContext res)
        {
            if (res == null || string.IsNullOrWhiteSpace(res.StringContent) || strategy == EnumCacheStrategy.ForceNoCache)
            {
                if (funcAsync == null)
                    throw new Exception("You must to specify the function that results a string");

                try
                {
                    res = await funcAsync();
                    res.RequestUrl = url;
                    res.HasUsedCache = false;

                    if (strategy == EnumCacheStrategy.CacheIfNotExist)
                        await this.SetCacheItemAsync(url, res);
                }
                catch (Exception ex)
                {
                    res = new ResponseContext();
                    res.RequestUrl = url;
                    res.HasUsedCache = false;
                    res.Exception = ex;
                }
            }

            return res;
        }

        public async Task<ResponseContext> UseCachedAsync(string url, Func<Task<ResponseContext>> funcAsync, EnumCacheStrategy strategy = EnumCacheStrategy.CacheIfNotExist)
        {
            ResponseContext res = await this.GetCacheResponseContextAsync(url);
            return await CachedAsync(url, funcAsync, strategy, res);
        }

        public async Task<ResponseContext> UseCachedAsync(string url, string postData, Func<Task<ResponseContext>> funcAsync, EnumCacheStrategy strategy = EnumCacheStrategy.CacheIfNotExist)
        {
            ResponseContext res = await this.GetCacheResponseContextAsync(url, postData);
            return await CachedAsync(url, funcAsync, strategy, res);
        }

        public async Task<ResponseContext> UseCachedAsync(string url, IEnumerable<KeyValuePair<string, string>> postData, Func<Task<ResponseContext>> funcAsync, EnumCacheStrategy strategy = EnumCacheStrategy.CacheIfNotExist)
        {
            ResponseContext res = await this.GetCacheResponseContextAsync(url, postData);
            return await CachedAsync(url, funcAsync, strategy, res);
        }

        public async Task<bool> SetCacheItemAsync(string url, ResponseContext content, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            var key = KeyHelper.GetKey(nameof(ResponseContext), url, postData);
            return await cache.SetAsync<ResponseContext>(key, content, duration);
        }

        public async Task<bool> SetCacheItemAsync(string url, ResponseContext content, string postData = null)
        {
            var key = KeyHelper.GetKey(nameof(ResponseContext), url, postData);
            return await cache.SetAsync<ResponseContext>(key, content, duration);
        }

        public async Task<bool> SetCacheItemAsync(string url, ResponseContext content)
        {
            var key = KeyHelper.GetKey(nameof(ResponseContext), url);
            return await cache.SetAsync<ResponseContext>(key, content, duration);
        }

        public async Task<bool> DeleteCacheItemAsync(string url, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            var key = KeyHelper.GetKey(nameof(ResponseContext), url, postData);
            return await cache.RemoveAsync(key);
        }
    }
}
