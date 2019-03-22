using HttpRequester;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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

            //var google = await cached.GetAsync("https://www.google.com");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}