using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequester
{
    public class ResponseContext
    {
        public string RequestUrl { get; set; }
        public string ResponseUrl { get; set; }

        public Exception Exception { get; set; }
        public bool HasUsedCache { get; set; }
        public string StringContent { get; set; }

        public bool HasErrors { get { return (this.Exception != null); } }

        public ResponseContext()
        {

        }

        public void SetContent(object obj)
        {
            if (typeof(object) == typeof(string))
            {
                this.StringContent = obj as string;

                // Json content
            }
        }
    }
}
