using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
    public interface IDriverRequester
    {
        Task<ResponseContext> GetAsync(string url);
        string LastCookie { get; }
        void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        void SetHeader(string key, string value);
        void SetUserAgent(string userAgent);
        void SetAcceptLanguage(string acceptLanguage);
        void SetCookie(string cookie);
        Task<string> GetContentAsync(string url);
        Task<string> PostContentAsync(string url, IEnumerable<KeyValuePair<string, string>> postData);
        Task<string> PostContentAsync(string url, string postData);
        Task<byte[]> DownloadDataTaskAsync(string url);
        Task<ResponseContext> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> postData);
        Task<ResponseContext> PostAsync(string url, string postData);
    }
}
