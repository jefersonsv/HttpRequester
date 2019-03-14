using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;
using PuppeteerSharp;
using System.Dynamic;

namespace HttpRequester.Engine
{
    public class ChromePersistentClient : ChromeBase
    {
        string url;
        Browser browser;
        public Page page;

        // https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        public ChromePersistentClient(bool headless)
        {
            // Starting
            browser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = base.FindChromeExecutable(),
                Headless = headless,
                Args = new string[] { "--incognito", "--disable-extensions", "--safe-plugins", "--disable-translate" }
            }).Result;

            page = browser.PagesAsync().Result.First();
        }

        public async Task CloseAsync()
        {
            await page.Browser.CloseAsync();
        }

        public async Task<Response> GoToAsync(string url)
        {
            this.url = url;
            return await page.GoToAsync(url);
        }

        public void WaitForSelector(string selector)
        {
            page.WaitForSelectorAsync(selector).Wait();
        }

        public async Task<string> GetContentAsync(string url)
        {
            await GoToAsync(url);
            return await GetSourceCodeAsync();
        }

        public async Task<string> GetSourceCodeAsync()
        {
            return await page.GetContentAsync();
        }

        public async Task TypeAsync(string selector, string text)
        {
            await page.WaitForSelectorAsync(selector);
            await page.TypeAsync(selector, text);
        }

        public async Task ClickAsync(string selector)
        {
            await page.WaitForSelectorAsync(selector);
            await page.ClickAsync(selector);
        }

        public void WaitNetworks()
        {
            NavigationOptions n = new NavigationOptions();
            n.WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle2 };

            page.WaitForNavigationAsync(n).Wait();
        }

        public IEnumerable<KeyValuePair<string, string>> PagesContent()
        {
            //var userAgent = this.browser.GetUserAgentAsync().Result;
            //var version = this.browser.GetVersionAsync().Result;
            //var last = this.browser.Targets().ToList().Last().PageAsync().Result;

            var pages = this.browser.PagesAsync().Result;
            foreach (var item in pages)
            {
                yield return new KeyValuePair<string, string>(item.Url, item.GetContentAsync().Result);
            }
        }

        public async Task<Newtonsoft.Json.Linq.JToken> ListCookiesAsync()
        {
            var cookies = await page.GetCookiesAsync(url);

            var obj = cookies.Select(s => new
            {
                Domain = s.Domain,
                Expires = s.Expires,
                HttpOnly = s.HttpOnly,
                Name = s.Name,
                Path = s.Path,
                Secure = s.Secure,
                Session = s.Session,
                Size = s.Size,
                Url = s.Url,
                Value = s.Value
            });

            return Newtonsoft.Json.Linq.JArray.FromObject(obj);
        }
    }
}
