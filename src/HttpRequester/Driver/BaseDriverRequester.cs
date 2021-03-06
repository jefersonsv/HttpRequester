﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequester.Driver
{
    public abstract class BaseDriverRequester
    {
        public string LastCookie { get; set; }
        public abstract void SetHeader(string key, string value);

        public abstract void RemoveHeader(string key);

        public void SetAcceptLanguage(string acceptLanguage)
        {
            SetHeader("Accept-Language", acceptLanguage);
        }

        public void SetAccept(string accept)
        {
            SetHeader("Accept", accept);
        }

        public void SetCookie(string cookie)
        {
            SetHeader("Cookie", cookie);
        }

        public void SetReferer(string referer)
        {
            SetHeader("Referer", referer);
        }

        /// <summary>
        /// Add or change Http Headers
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/List_of_HTTP_header_fields#Accept-Language"/>
        /// <param name="headers"></param>
        public void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var item in headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
                this.SetHeader(item.Key, item.Value);
        }

        public void SetUserAgent(string userAgent)
        {
            SetHeader("User-Agent", userAgent);
        }
    }
}
