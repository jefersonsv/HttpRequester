using HttpRequester;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpCached
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CachedRequester cached = new CachedRequester(new Requester(EnumHttpProvider.HttpClient), duration: new TimeSpan(1, 0, 0));
            var task = await cached.GetContentAsync("https://www.google.com", "");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
