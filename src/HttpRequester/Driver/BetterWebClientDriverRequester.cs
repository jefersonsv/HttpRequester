using HttpRequester.Engine;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
    /// <summary>
    /// This driver preserv cookie container and don't preserv referer
    /// </summary>
    public class BetterWebClientDriverRequester : BaseDriverRequester, IDriverRequester
    {
        BetterWebClient client = null;

        public BetterWebClientDriverRequester()
        {
            this.client = new BetterWebClient();
            this.client.AutoRedirect = false;
        }

        public async Task<byte[]> DownloadDataTaskAsync(string url)
        {
            var res = await client.DownloadDataTaskAsync(url);
            LastCookie = this.client.CookieContainer.GetCookieHeader(new Uri(url));
            return res;
        }

        public async Task<ResponseContext> GetAsync(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetContentAsync(string url)
        {
            var res = await client.DownloadStringTaskAsync(new Uri(url));
            LastCookie = this.client.CookieContainer.GetCookieHeader(new Uri(url));
            return res;
        }

        public async Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var nameValueCollection = new NameValueCollection();
            postData.ToList().ForEach(a => nameValueCollection.Add(a.Key, a.Value));

            var res = await this.client.UploadValuesTaskAsync(url, "POST", nameValueCollection);
            LastCookie = this.client.CookieContainer.GetCookieHeader(new Uri(url));
            return Encoding.Default.GetString(res);
        }

        public async Task<string> PostContentAsync(string url, string postData)
        {
            var nameValueCollection = new NameValueCollection();
            var res = await this.client.UploadStringTaskAsync(url, "POST", postData);
            LastCookie = this.client.CookieContainer.GetCookieHeader(new Uri(url));

            return res;
        }

        public override void SetHeader(string key, string value)
        {
            client.Headers[key] = value;
        }
    }
}
