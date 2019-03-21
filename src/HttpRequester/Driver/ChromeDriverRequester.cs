using AngleSharp;
using HttpRequester.Engine;
using System;
using System.Collections.Generic;
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
    public class ChromeDriverRequester : BaseDriverRequester, IDriverRequester
    {
        ChromeClient client = null;

        public ChromeDriverRequester()
        {
            this.client = new ChromeClient(true);
        }

        public async Task<string> GetContentAsync(string url)
        {
            var res = await client.GetContentAsync(url);
            return res;
        }

        public override void SetHeader(string key, string value)
        {
            throw new Exception("You cannot modify header using chrome headless because the browser will manage it for you");
        }

        public async Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            throw new NotImplementedException();
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
