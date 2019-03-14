using Foundatio.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequester
{
    internal class HttpRequest
    {
        ICacheClient cache = null;

        internal HttpRequest(EnumHttpProvider httpRequest, string endPointRedisCache = null, string passwordRedisCache = null)
        {
            if (!string.IsNullOrEmpty(endPointRedisCache))
            {
                cache = CacheClient.GetConnection(endPointRedisCache, passwordRedisCache);
            }
        }
    }
}
