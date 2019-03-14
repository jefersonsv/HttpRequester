using AngleSharp;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
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
    }
}
