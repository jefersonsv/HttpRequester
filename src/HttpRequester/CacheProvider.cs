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

        public CacheProvider(DataFoundation.Redis.RedisConnection redis, TimeSpan? duration = null)
        {
            this.duration = duration ?? TimeSpan.FromDays(30);
            cache = new RedisCacheClient(o => o.ConnectionMultiplexer(redis.Client));
        }

        static string GetKey(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            string hash = null;
            if (postData != null && postData.Any())
            {
                var ordered = postData.OrderBy(o => o.Key).ThenBy(o => o.Value).ToList();
                JArray arr = JArray.FromObject(ordered);
                var json = arr.ToString();
                hash = MD5Base64Hash(json);
            }

            UriBuilder uri = new UriBuilder(new Uri(url.ToLower()));
            uri.Password = null;

            if (uri.Scheme.ToLower().Trim() == "http" && uri.Port == 80)
                uri.Port = -1;

            if (uri.Scheme.ToLower().Trim() == "https" && uri.Port == 443)
                uri.Port = -1;

            var cleaned = uri.ToString().Replace('/', ':').Replace('.', ':');
            if (!string.IsNullOrEmpty(hash))
                cleaned = cleaned + ":#" + hash;

            cleaned = AllReplace(cleaned, ":").TrimEnd(':');
            return cleaned;
        }

        static string AllReplace(string text, string character)
        {
            Regex regex = new Regex(character + "{2,}");
            return regex.Replace(text, character);
        }

        static string MD5AsciiHash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return Encoding.ASCII.GetString(result);
            }
        }

        static string MD5Base64Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return Base64Encode(Encoding.ASCII.GetString(result));
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
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
