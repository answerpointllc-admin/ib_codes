
using Microsoft.AspNetCore.Http;

namespace IoTFeeder.Common.Helper
{
    public static class CookieExtensions
    {
        public static string GetObject(this IRequestCookieCollection cookie, string key)
        {
            var value = cookie.GetObject(key);
            return value;
        }
    }
}
