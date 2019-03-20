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

        static string GetKeyWithKeyValuePostData(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var key = GetKeyWithUrl(url);

            string hash = null;
            if (postData != null && postData.Any())
            {
                var ordered = postData.OrderBy(o => o.Key).ThenBy(o => o.Value).ToList();
                JArray arr = JArray.FromObject(ordered);
                var json = arr.ToString();
                hash = MD5Base64Hash(json);

                return key + ":#" + hash;
            }

            return key;
        }

        static string GetKeyWithStringPostData(string url, string postData)
        {
            var key = GetKeyWithUrl(url);

            if (postData != null && postData.Any())
            {
                return key + ":#" + MD5Base64Hash(postData);
            }

            return key;
        }

        static string GetKeyWithUrl(string url)
        {
            UriBuilder uri = new UriBuilder(new Uri(url.ToLower()));
            uri.Password = null;

            if (uri.Scheme.ToLower().Trim() == "http" && uri.Port == 80)
                uri.Port = -1;

            if (uri.Scheme.ToLower().Trim() == "https" && uri.Port == 443)
                uri.Port = -1;

            var cleaned = uri.ToString().Replace('/', ':').Replace('.', ':');

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

        public async Task<string> GetCacheAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var key = GetKeyWithKeyValuePostData(url, postData);

            var cached = await cache.GetAsync<string>(key);
            return cached.Value;
        }

        public async Task<string> GetCacheAsync(string url, string postData)
        {
            var key = GetKeyWithStringPostData(url, postData);

            var cached = await cache.GetAsync<string>(key);
            return cached.Value;
        }

        public async Task<string> GetCacheAsync(string url)
        {
            var key = GetObjectKey(url);

            var cached = await cache.GetAsync<string>(key);
            return cached.Value;
        }

        public string GetObjectKey(string url, string objectName = null)
        {
            return string.IsNullOrEmpty(objectName) ? 
                GetKeyWithUrl(url) : 
                $"{objectName.Humanize().Replace(' ', '-').ToLower()}:" + GetKeyWithUrl(url);
        }

        public async Task<ResponseContext> GetCacheResponseContextAsync(string url)
        {
            var key = GetObjectKey(url, nameof(ResponseContext));

            var cached = await cache.GetAsync<ResponseContext>(key);
            return cached.Value;
        }

        public async Task<ResponseContext> UseCachedAsync(string url, Func<Task<ResponseContext>> funcAsync)
        {
            var res = new ResponseContext();
            res.RequestUrl = url;

            var stringContent = await this.GetCacheResponseContextAsync(url);
            if (stringContent == null || string.IsNullOrEmpty(stringContent.StringContent))
            {
                if (funcAsync == null)
                    throw new Exception("You must to specify the function that results a string");

                res.HasUsedCache = false;
                try
                {
                    res = await funcAsync();
                    await this.SetCacheAsync(url, res);
                }
                catch (Exception ex)
                {
                    res.StringContent = null;
                    res.Exception = ex;
                }

                return res;
            }

            stringContent.HasUsedCache = true;
            return stringContent;
        }

        public async Task<ResponseContext> UseCachedAsync(string url, Func<Task<string>> funcAsync)
        {
            var res = new ResponseContext();
            res.RequestUrl = url;

            var stringContent = await this.GetCacheAsync(url);

            if (string.IsNullOrEmpty(stringContent))
            {
                if (funcAsync == null)
                    throw new Exception("You must to specify the function that results a string");

                res.HasUsedCache = false;
                try
                {
                    res.StringContent = await funcAsync();
                    await this.SetCacheAsync(url, res.StringContent);
                }
                catch (Exception ex)
                {
                    res.StringContent = null;
                    res.Exception = ex;
                }
                return res;
            }

            res.HasUsedCache = true;
            res.StringContent = stringContent;
            return res;
        }

        public async Task<ResponseContext> UseCachedAsync(string url, IEnumerable<KeyValuePair<string, string>> postData, Func<Task<string>> funcAsync)
        {
            var res = new ResponseContext();
            res.RequestUrl = url;

            var stringContent = await this.GetCacheAsync(url, postData);

            if (string.IsNullOrEmpty(stringContent))
            {
                if (funcAsync == null)
                    throw new Exception("You must to specify the function that results a string");

                res.HasUsedCache = false;
                try
                {
                    res.StringContent = await funcAsync();
                    await this.SetCacheAsync(url, res.StringContent, postData);
                }
                catch (Exception ex)
                {
                    res.StringContent = null;
                    res.Exception = ex;
                }
                return res;
            }

            res.HasUsedCache = true;
            res.StringContent = stringContent;
            return res;
        }

        public async Task<ResponseContext> UseCachedAsync(string url, string postData, Func<Task<string>> funcAsync)
        {
            var res = new ResponseContext();
            res.RequestUrl = url;

            var stringContent = await this.GetCacheAsync(url, postData);
            if (string.IsNullOrEmpty(stringContent))
            {
                if (funcAsync == null)
                    throw new Exception("You must to specify the function that results a string");

                res.HasUsedCache = false;
                try
                {
                    res.StringContent = await funcAsync();
                    await this.SetCacheAsync(url, res.StringContent, postData);
                }
                catch (Exception ex)
                {
                    res.StringContent = null;
                    res.Exception = ex;
                }
                return res;
            }

            res.HasUsedCache = true;
            res.StringContent = stringContent;
            return res;
        }

        public async Task<bool> SetCacheAsync(string url, string content, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var key = GetKeyWithKeyValuePostData(url, postData);
            return await cache.SetAsync<string>(key, content, duration);
        }

        public async Task<bool> SetCacheAsync(string url, string content, string postData)
        {
            var key = GetKeyWithStringPostData(url, postData);
            return await cache.SetAsync<string>(key, content, duration);
        }

        public async Task<bool> SetCacheAsync(string url, string content)
        {
            var key = GetObjectKey(url);
            return await cache.SetAsync<string>(key, content, duration);
        }

        public async Task<bool> SetCacheAsync(string url, ResponseContext content)
        {
            var key = GetObjectKey(url, nameof(ResponseContext));
            return await cache.SetAsync<ResponseContext>(key, content, duration);
        }

        public async Task<bool> DeleteCacheAsync(string url, IEnumerable<KeyValuePair<string, string>> postData = null)
        {
            var key = GetKeyWithKeyValuePostData(url, postData);
            return await cache.RemoveAsync(key);
        }
    }
}
