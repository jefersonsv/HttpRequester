using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
    /// <summary>
    /// This drivers preserv Referer http header after first request
    /// </summary>
    public class HttpClientDriverRequester : DriverRequester, IDriverRequester
    {
        HttpClient client = null;

        public HttpClientDriverRequester()
        {
            this.client = new HttpClient();
        }

        public async Task<string> GetContentAsync(string url)
        {
            var res = await client.GetStringAsync(url);
            return res;
        }

        public async Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var body = new FormUrlEncodedContent(postData);
            var post = await this.client.PostAsync(url, body);
            return await post.Content.ReadAsStringAsync();
        }

        public override void SetHeader(string key, string value)
        {
            if (this.client.DefaultRequestHeaders.Contains(key))
                this.client.DefaultRequestHeaders.Remove(key);

            this.client.DefaultRequestHeaders.Add(key, value);
        }
    }
}
