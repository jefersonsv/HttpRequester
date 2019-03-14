using Foundatio.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpRequester
{
    public static class CacheClient
    {
        public static ICacheClient GetConnection(string endPointRedisCache, string passwordRedisCache = null)
        {
            var opt = new ConfigurationOptions();
            opt.EndPoints.Add(endPointRedisCache);

            if (!string.IsNullOrEmpty(passwordRedisCache))
            opt.Password = passwordRedisCache;

            return new RedisCacheClient(o => o.ConnectionMultiplexer(ConnectionMultiplexer.Connect(opt)));
        }

        public static string GetCacheKey(string url, string body = null)
        {
            UriBuilder uri = new UriBuilder(new Uri(url));
            uri.Password = null;

            var u = uri.ToString().Replace('/', ':').Replace('.', ':');
            var template = AllReplace(u, ":").TrimEnd(':');
            if (!string.IsNullOrEmpty(body))
                template = template + "#" + MD5Hash(body);

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
    }
}
