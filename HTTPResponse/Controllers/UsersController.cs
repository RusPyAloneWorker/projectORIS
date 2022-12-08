using System;
using System.Collections.Generic;
using System.Linq;
using HTTPResponse.Attributes;
using System.Net;
using HTTPResponse.Models.UserModel;
using HTTPResponse;
using HTTPResponse.Context;
using RazorEngine;
using RazorEngine.Templating;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text;
using RazorGeneratorLibrary;
using System.Data.SqlClient;


namespace HTTPResponse.Controllers
{
    [HttpController("users")]
    internal class UsersController
    {
        private static User _user = new User();
        private static string _path = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllText("Config.json")).Path;
        public UsersController() { }

        [HttpGET("get_user", true)]
        public async Task GetUserById (HttpListenerContext context)
        {      
            string[] strParams;
            HttpHandler.HandleGET(context, out strParams);
            int id;

            if (strParams.Length != 1 || !Int32.TryParse(strParams[0], out id))
                throw new ArgumentException();

            var user = _user.FindById(id);

            // Headers
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");
            context.Response.Headers = headers;

            string html = FileFinder.GetFileStrings("/html/get_user.html", _path);
            string template = RazorGenerator<User>.Run("get_user", html, user);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(template);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        [HttpGET("get_users", true)]
        public async Task GetUsers(HttpListenerContext context)
        {
            List<User> users = new List<User>();
            users = _user.FindAll();

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");
            context.Response.Headers = headers;

            string html = FileFinder.GetFileStrings("/html/get_users.html", _path);
            string template = RazorGenerator<List<User>>.Run("get_users", html, users);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(template);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        [HttpPOST("save_user")]
        public async Task SaveUser(HttpListenerContext context) 
        {
            Dictionary<string,string> strParams;
            HttpHandler.HandlePOST(context, out strParams);

            if (strParams is null)
                throw new ArgumentException();

            var user = _user.InsertAndGiveBack(new User(email: strParams["email"], password: strParams["password"]));

            if (user is not null)
            {
                CookieManager.AddCookie(context, "SessionId", user.user_id.ToString(), 20d);
                context.Response.Redirect($@"http://localhost:8080/users/get_user/{user.user_id}");
            }
            else
            {
                WebHeaderCollection headers = new WebHeaderCollection();
                headers.Set("Content-Type", "text/css");
                headers.Add("Content-Type", "text/html");
                context.Response.Headers = headers;

                string template = "Email or password is already taken.";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(template);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }
        [HttpPOST("sing_up_user")]
        public async Task Login(HttpListenerContext context)
        {
            var strParams = new Dictionary<string, string>();
            HttpHandler.HandlePOST(context, out strParams);
            if (!(strParams is null || strParams.Count == 0))
            {
                var user = _user.FindByEmailAndPassword(email: strParams["email"], password: strParams["password"]);

                if (user is null)
                {
                    context.Response.StatusCode = 400;

                    WebHeaderCollection headers = new WebHeaderCollection();
                    headers.Set("Content-Type", "text/css");
                    headers.Add("Content-Type", "text/html");

                    context.Response.Headers = headers;

                    string template = "Email or password is incorrect!";
                    byte[] buffer = System.Text.Encoding.UTF32.GetBytes(template);
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    WebHeaderCollection headers = new WebHeaderCollection();
                    headers.Set("Content-Type", "text/css");
                    headers.Add("Content-Type", "text/html");
                    
                    context.Response.Headers = headers; 
                    CookieManager.AddCookie(context, "SessionId", user.user_id.ToString(), 20d);
                    //string template = Uri.UnescapeDataString($@"{user.email}, {user.name}");
                    //byte[] buffer = System.Text.Encoding.UTF32.GetBytes(template);
                    //context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    context.Response.Redirect($@"http://localhost:8080/users/get_user/{user.user_id}");
                }
            }
            else
            {
                byte[] buffer = System.Text.Encoding.UTF32.GetBytes("Incorrect input data");
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 400;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }

    }
}
