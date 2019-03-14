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
            var h = new HttpRequester.HttpRequest(EnumHttpProvider.HttpClient, new CacheProvider("127.0.0.1", "bHEHe99cuLMRsGpzVerqC3Pa6hH06Ic9cq7peYH5jNitYidBktjzD62piTQCAKWq"));
            var sdaadsdfsdfdsf = await h.GetContentAsync("http://getip.wholecheap.com");

            var r = new HttpClientDriverRequester();
            //r.SetUserAgent("cACIQUE");
            //r.SetAcceptLanguage("en-US");

            
            //r.SetUserAgent("cACIQUE 23432");
            r.GetContentAsync("https://www.google.com").Wait();

            return;
            

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
;