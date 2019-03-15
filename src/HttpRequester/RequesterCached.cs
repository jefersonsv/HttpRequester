using Foundatio.Caching;
using HttpRequester.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester
{
    public class RequesterCached : IDriverRequester
    {
        readonly CacheProvider cacheProvider;
        readonly IDriverRequester driver;
        public string LastCookie { get => driver.LastCookie; }

        public RequesterCached(EnumHttpProvider httpRequest, CacheProvider cacheProvider = null)
        {
            switch (httpRequest)
            {
                case EnumHttpProvider.AngleSharp:
                    driver = new AngleSharpDriverRequester();
                    break;

                case EnumHttpProvider.BetterWebClient:
                    driver = new BetterWebClientDriverRequester();
                    break;

                case EnumHttpProvider.ChromeHeadless:
                    driver = new ChromeDriverRequester();
                    break;

                case EnumHttpProvider.ChromeHeadlessPersistent:
                    driver = new ChromePersistentDriverRequester();
                    break;

                case EnumHttpProvider.HttpClient:
                    driver = new HttpClientDriverRequester();
                    break;

                case EnumHttpProvider.WebClient:
                    driver = new WebClientDriverRequester();
                    break;
            }
            this.cacheProvider = cacheProvider;
        }

        public async Task<byte[]> DownloadDataTaskAsync(string url)
        {
            return await this.driver.DownloadDataTaskAsync(url);
        }

        public async Task<string> GetContentAsync(string url)
        {
            Func<Task<string>> call = async () => await driver.GetContentAsync(url);
            return cacheProvider == null ? await call() : await cacheProvider.UseCachedAsync(url, (string) null, call);
        }

        public async Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            Func<Task<string>> call = async () => await driver.PostContentAsync(url, postData);
            return cacheProvider == null ? await call() : await cacheProvider.UseCachedAsync(url, postData, call);
        }

        public async Task<string> PostContentAsync(string url, string postData)
        {
            Func<Task<string>> call = async () => await driver.PostContentAsync(url, postData);
            return cacheProvider == null ? await call() : await cacheProvider.UseCachedAsync(url, postData, call);
        }

        public void SetAcceptLanguage(string acceptLanguage)
        {
            driver.SetAcceptLanguage(acceptLanguage);
        }

        public void SetCookie(string cookie)
        {
            driver.SetAcceptLanguage(cookie);
        }

        public void SetHeader(string key, string value)
        {
            driver.SetHeader(key, value);
        }

        public void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            driver.SetHeaders(headers);
        }

        public void SetUserAgent(string userAgent)
        {
            driver.SetUserAgent(userAgent);
        }
    }
}
