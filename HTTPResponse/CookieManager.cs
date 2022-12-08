using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json;

namespace HTTPResponse
{
    public static class CookieManager
    {
        public static void AddCookie(HttpListenerContext context, string name, string value, double lifetime)
        {
            context.Response.Cookies.Add(new Cookie
            {
                Name = name,
                Value = value,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(lifetime)
            });
            Console.WriteLine(JsonSerializer.Serialize<object>(value));
        }
        public static Cookie GetCookie(HttpListenerContext context, string name)
        {
            return context.Request.Cookies.Where(t => t.Name == name).FirstOrDefault();
        }
        public static bool IsAuthorized(HttpListenerContext context)
        {
            var cookie = GetCookie(context, "SessionId");
            if (cookie is null) return false;
            return true;
        }
    }
}
