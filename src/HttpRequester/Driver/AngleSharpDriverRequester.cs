using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
    /// <summary>
    /// if (Helper.IsNetworkAvailable())
//            {
//                var http = new DefaultHttpRequester();
//    var request = new Request
//    {
//        Address = new Url("http://httpbin.org/status/500"),
//        Method = HttpMethod.Get
//    };

//                using (var response = await http.RequestAsync(request, CancellationToken.None))
//                {
//                    Assert.IsNotNull(response);
//                    Assert.AreEqual(500, (int) response.StatusCode);
//    Assert.IsTrue(response.Content.CanRead);
//                    Assert.IsTrue(response.Headers.Count > 0);
//                }
//}
    /// </summary>
    /// <seealso cref="https://github.com/AngleSharp/AngleSharp/blob/master/src/AngleSharp.Core.Tests/Library/HttpRequester.cs"/>
    public class AngleSharpDriverRequester : BaseDriverRequester, IDriverRequester
    {
        IBrowsingContext client = null;
        DefaultHttpRequester defaultHttpRequester = null;

        public AngleSharpDriverRequester()
        {
            var configuration = GetBasicConfiguration();
            defaultHttpRequester = new DefaultHttpRequester();
            this.client = BrowsingContext.New(configuration);
        }

        IConfiguration GetBasicConfiguration()
        {
            AngleSharp.Io.LoaderOptions load = new AngleSharp.Io.LoaderOptions();
            load.IsNavigationDisabled = false;
            load.IsResourceLoadingEnabled = false;

            return Configuration.Default.WithDefaultLoader(load);
        }

        public async Task<string> GetContentAsync(string url)
        {
            var browse = await client.OpenAsync(url);
            return browse.Source.Text;
        }

        public async Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            throw new NotImplementedException();
        }

        public override void SetHeader(string key, string value)
        {
            //var configuration = GetBasicConfiguration();

            var requester = client.GetService<DefaultHttpRequester>();
            requester.Headers[key] = value;

            //client = BrowsingContext.New(configuration.With(requester));
        }

        public async Task<byte[]> DownloadDataTaskAsync(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<string> PostContentAsync(string url, string postData)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseContext> GetAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseContext> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseContext> PostAsync(string url, string postData)
        {
            throw new NotImplementedException();
        }
    }
}
