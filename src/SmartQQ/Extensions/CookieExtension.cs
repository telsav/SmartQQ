using SmartQQ.Constants;
using System.Net;

namespace SmartQQ
{
    public static class CookieExtension
    {
        public static CookieCollection GetCookies(this CookieContainer cookies, ApiUrl url, params object[] args)
        {
            return cookies.GetCookies(new System.Uri(url.BuildUrl(args)));
        }
    }
}
