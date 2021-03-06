﻿using AngleSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
    /// <summary>
    /// This driver don't preserv referer http header
    /// </summary>
    public class WebClientDriverRequester : BaseDriverRequester, IDriverRequester
    {
        WebClient client = null;

        public WebClientDriverRequester()
        {
            this.client = new WebClient();
        }

        public async Task<string> GetContentAsync(string url)
        {
            var res = await client.DownloadStringTaskAsync(new Uri(url));
            return res;
        }

        public async Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            var nameValueCollection = new NameValueCollection();
            postData.ToList().ForEach(a => nameValueCollection.Add(a.Key, a.Value));

            var res = await this.client.UploadValuesTaskAsync(url, "POST", nameValueCollection);
            //LastCookie = this.client.Container.co
            //Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);
            return Encoding.Default.GetString(res);
        }

        public async Task<string> PostContentAsync(string url, string postData)
        {
            var nameValueCollection = new NameValueCollection();
            var res = await this.client.UploadStringTaskAsync(url, "POST", postData);
            //LastCookie = this.client.Container.co
            //Cookies = betterWebClient.CookieContainer.GetCookieHeader(uri);

            return res;
        }

        public async Task<byte[]> DownloadDataTaskAsync(string url)
        {
            var bytes = await client.DownloadDataTaskAsync(new Uri(url));
            return bytes;
        }

        public override void SetHeader(string key, string value)
        {
            client.Headers[key] = value;
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

        public override void RemoveHeader(string key)
        {
            client.Headers.Remove(key);
        }
    }
}
