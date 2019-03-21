using Humanizer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HttpRequester
{
    public static class KeyHelper
    {
        public static string GetKey(string objectName, string url)
        {
            return ClearDoubleDots($"{RenderObjectNameKey(objectName)}:{RenderUrlKey(url)}");
        }

        public static string GetKey(string objectName, string url, string postData)
        {
            return ClearDoubleDots($"{RenderObjectNameKey(objectName)}:{RenderUrlKey(url)}:{RenderPostDataKey(postData)}");
        }

        public static string GetKey(string objectName, string url, IEnumerable<KeyValuePair<string, string>> postData)
        {
            return ClearDoubleDots($"{RenderObjectNameKey(objectName)}:{RenderUrlKey(url)}:{RenderKeyValueKey(postData)}");
        }

        static string RenderObjectNameKey(string objectName)
        {
            return objectName.Humanize().Replace(' ', '-').ToLower();
        }

        static string RenderPostDataKey(string postData)
        {
            return string.IsNullOrEmpty(postData) ? string.Empty : "#" + HashHelper.MD5Base64Hash(postData);
        }

        static string RenderKeyValueKey(IEnumerable<KeyValuePair<string, string>> postData)
        {
            if (postData != null && postData.Any())
            {
                var ordered = postData.OrderBy(o => o.Key).ThenBy(o => o.Value).ToList();
                JArray arr = JArray.FromObject(ordered);
                var json = arr.ToString();
                return "#" + HashHelper.MD5Base64Hash(json);
            }

            return string.Empty;
        }

        static string RenderUrlKey(string url)
        {
            UriBuilder uri = new UriBuilder(new Uri(url.ToLower()));
            uri.Password = null;

            if (uri.Scheme.ToLower().Trim() == "http" && uri.Port == 80)
                uri.Port = -1;

            if (uri.Scheme.ToLower().Trim() == "https" && uri.Port == 443)
                uri.Port = -1;

            var cleaned = uri.ToString().Replace('/', ':').Replace('.', ':');

            
            return cleaned;
        }

        static string ClearDoubleDots(string text)
        {
            return AllReplace(text, ":").TrimEnd(':').TrimStart(':');
        }

        static string AllReplace(string text, string character)
        {
            Regex regex = new Regex(character + "{2,}");
            return regex.Replace(text, character);
        }
    }
}
