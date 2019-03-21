using AngleSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
    /// <summary>
    /// This drivers preserv Referer http header after first request
    /// </summary>
    public class HttpClientDriverRequester : BaseDriverRequester, IDriverRequester
    {
        HttpClient client = null;

        public HttpClientDriverRequester()
        {
            this.client = new HttpClient();
        }

        public async Task<ResponseContext> GetAsync(string url)
        {
            ResponseContext res = new ResponseContext();
            res.RequestUrl = url;

            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                res.ResponseUrl = response.RequestMessage?.RequestUri?.ToString() ?? url;
                res.StringContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                res.Exception = ex;
            }

            return res;
        }

        public async Task<ResponseContext> PostAsync(string url, string postData)
        {
            ResponseContext res = new ResponseContext();
            res.RequestUrl = url;

            try
            {
                var response = await client.PostAsync(url, new StringContent(postData));
                response.EnsureSuccessStatusCode();

                res.ResponseUrl = response.RequestMessage?.RequestUri?.ToString() ?? url;
                res.StringContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                res.Exception = ex;
            }

            return res;
        }

        public async Task<ResponseContext> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            ResponseContext res = new ResponseContext();
            res.RequestUrl = url;

            try
            {
                var body = new FormUrlEncodedContent(postData);
                var response = await this.client.PostAsync(url, body);
                response.EnsureSuccessStatusCode();

                res.ResponseUrl = response.RequestMessage?.RequestUri?.ToString() ?? url;
                res.StringContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                res.Exception = ex;
            }

            return res;
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

        public async Task<string> PostContentAsync(string url, string postData)
        {
            var post = await this.client.PostAsync(url, new StringContent(postData));
            return await post.Content.ReadAsStringAsync();
        }

        public override void SetHeader(string key, string value)
        {
            if (key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (key.Equals("Connection", StringComparison.InvariantCultureIgnoreCase))
            {
                this.client.DefaultRequestHeaders.Remove("Connection");
                this.client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                return;
            }

            if (this.client.DefaultRequestHeaders.Contains(key))
                this.client.DefaultRequestHeaders.Remove(key);

            this.client.DefaultRequestHeaders.Add(key, value);
        }

        public async Task<byte[]> DownloadDataTaskAsync(string url)
        {
            var res = await client.GetByteArrayAsync(url);
            return res;
        }
    }
}
