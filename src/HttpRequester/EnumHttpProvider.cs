﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequester
{
    public enum EnumHttpProvider
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
