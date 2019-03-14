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
    public class ChromePersistentDriverRequester : DriverRequester, IDriverRequester
    {
        ChromePersistentClient client = null;

        public ChromePersistentDriverRequester()
        {
            this.client = new ChromePersistentClient(true);
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
    }
}
