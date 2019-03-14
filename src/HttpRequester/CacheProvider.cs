using Foundatio.Caching;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
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

        public CacheProvider(string endPointRedisCache, string passwordRedisCache = null, TimeSpan? duration = null)
        {
            var opt = new ConfigurationOptions();
            opt.EndPoints.Add(endPointRedisCache);

            if (!string.IsNullOrEmpty(passwordRedisCache))
            opt.Password = passwordRedisCache;

            this.duration = duration ?? TimeSpan.FromDays(30);

            cache = new RedisCacheClient(o => o.ConnectionMultiplexer(ConnectionMultiplexer.Connect(opt)));
        }

        static string GetKey(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            string hash = null;
            if (postData != null && postData.Any())
            {
                var ordered = postData.OrderBy(o => o.Key).ThenBy(o => o.Value).ToList();
                JArray arr = JArray.FromObject(ordered);
                var json = arr.ToString();
                hash = MD5Hash(json);
            }

            UriBuilder uri = new UriBuilder(new Uri(url));
            uri.Password = null;

            var u = uri.ToString().Replace('/', ':').Replace('.', ':');
            var template = AllReplace(u, ":").TrimEnd(':');
            if (!string.IsNullOrEmpty(hash))
                template = template + "#" + hash;

            return template;
        }

        static string AllReplace(string text, string character)
        {
            Regex regex = new Regex(character + "{2,}");
            return regex.Replace(text, character);
        }

        static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return Encoding.ASCII.GetString(result);
            }
        }

        public async Task<string> GetCachedAsync(string url, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            var key = GetKey(url, postData);

            var cached = await cache.GetAsync<string>(key);
            return cached.Value;
        }

        public async Task<bool> SetCacheAsync(string url, string content, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            var key = GetKey(url, postData);
            return await cache.SetAsync<string>(key, content, duration);
        }
    }
}
