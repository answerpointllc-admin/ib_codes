using IoTFeeder.Common.Common;
using Microsoft.AspNetCore.Http;
using System;

namespace IoTFeeder.Common.Helper
{
    public class CookieManager
    {
        private static HttpContextAccessor _HttpContextAccessor = new HttpContextAccessor();

        public static void SetCookie(string keyName, string value, CookieOptions cookieOptions = null)
        {
            if (cookieOptions != null)
                _HttpContextAccessor.HttpContext.Response.Cookies.Append(keyName, value, cookieOptions);
            else
                _HttpContextAccessor.HttpContext.Response.Cookies.Append(keyName, value, new CookieOptions { Expires = DateTime.UtcNow.AddMinutes(GlobalCode.CacheTime) });
        }

        public static string GetCookie(string keyName)
        {
            return _HttpContextAccessor.HttpContext.Request.Cookies.GetObject(keyName);
        }

        public static void DeleteCookie(string keyName)
        {
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(keyName);
        }
    }
}
