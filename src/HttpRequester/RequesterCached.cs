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

        public async Task<string> GetContentAsync(string url)
        {
            if (cacheProvider != null)
            {
                var content = await cacheProvider.GetCachedAsync(url);
                if (string.IsNullOrEmpty(content))
                {
                    content = await driver.GetContentAsync(url);
                    await cacheProvider.SetCacheAsync(url, content);
                }

                return content;
            }
            else
                return await driver.GetContentAsync(url);
        }

        public async Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            if (cacheProvider != null)
            {
                var content = await cacheProvider.GetCachedAsync(url, postData);
                if (string.IsNullOrEmpty(content))
                {
                    content = await driver.PostContentAsync(url, postData);
                    await cacheProvider.SetCacheAsync(url, content, postData);
                }

                return content;
            }
            else
                return await driver.PostContentAsync(url, postData);
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
