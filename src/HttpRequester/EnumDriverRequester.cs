using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequester
{
    public enum EnumDriverRequester
    {
        HttpClient,
        AngleSharp,
        WebClient,
        BetterWebClient,

        /// <summary>
        /// Best used to SPA
        /// </summary>
        ChromeHeadless,

        ChromeHeadlessPersistent,
    }
}
