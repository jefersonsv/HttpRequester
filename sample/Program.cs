using Foundatio.Caching;
using HttpRequester;
using HttpRequester.Driver;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace HttpCached
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var cached = new HttpRequester.RequesterCached(EnumHttpProvider.HttpClient, new CacheProvider(new DataFoundation.Redis.RedisConnection()));
            var normal = new HttpRequester.RequesterCached(EnumHttpProvider.HttpClient);

            var google = await cached.GetContentAsync("https://www.google.com");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
;