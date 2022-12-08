using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPResponse.Attributes;
using HTTPResponse;
using HTTPResponse.Context;
using RazorEngine;
using RazorEngine.Templating;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.Json;
using RazorGeneratorLibrary;

namespace HTTPResponse.Controllers
{
    [HttpController("HomePage")]
    class HomePageController
    {
        private static string _path = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllText("Config.json")).Path;

        [HttpGET("show_homepage")]
        public void ShowHomePage(HttpListenerContext context)
        {
            // headers
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");

            // generating template
            string html = FileFinder.GetFileStrings("/index.html", _path);
            string template = RazorGenerator<object>.Run("get_user", html, new { MethodName = "save_user" });

            //buffer
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(template);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        [HttpGET("sign_up")]
        public void Sign_Up(HttpListenerContext context)
        {
            // headers
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");

            // generating template
            string html = FileFinder.GetFileStrings("/index.html", _path);
            string template = RazorGenerator<object>.Run("get_user", html, new { MethodName = "sing_up_user" });

            // buffer
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(template);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
