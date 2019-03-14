using Foundatio.Caching;
using HttpRequester;
using HttpRequester.Driver;
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
            var r = new HttpClientDriverRequester();
            //r.SetUserAgent("cACIQUE");
            //r.SetAcceptLanguage("en-US");

            var c = r.GetContentAsync("http://getip.wholecheap.com").Result;
            //r.SetUserAgent("cACIQUE 23432");
            r.GetContentAsync("https://www.google.com").Wait();

            return;
            var opt = new ConfigurationOptions();
            opt.EndPoints.Add("127.0.0.1");
            opt.Password = "bHEHe99cuLMRsGpzVerqC3Pa6hH06Ic9cq7peYH5jNitYidBktjzD62piTQCAKWq";

            ICacheClient cache = new RedisCacheClient(o => o.ConnectionMultiplexer(StackExchange.Redis.ConnectionMultiplexer.Connect(opt)));


            await cache.SetAsync("test", 1);
            var value = await cache.GetAsync<int>("test");

            return;
            CachedRequester cached = new CachedRequester(new Requester(EnumHttpProvider.HttpClient), duration: new TimeSpan(1, 0, 0));
            var task = await cached.GetContentAsync("https://www.google.com", "");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
;