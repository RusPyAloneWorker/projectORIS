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
using RazorGeneratorLibrary;


namespace HTTPResponse.Controllers
{
    [HttpController("users")]
    internal class UsersController
    {
        private static User _user = new User();
        private static string _path = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllText("Config.json")).Path;
        public UsersController() { }

        [HttpGET("get_user")]
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

        [HttpGET("get_users")]
        public async Task GetUsers(HttpListenerContext context)
        {
            List<User> users = new List<User>();
            users = _user.FindAll();

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");

            string html = FileFinder.GetFileStrings("/html/get_users.html", _path);
            string template = RazorGenerator<List<User>>.Run("get_users", html, users);

            context.Response.Headers = headers;

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(template);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        [HttpPOST("save_user")]
        public void SaveUser(HttpListenerContext context) 
        {
            Dictionary<string,string> strParams;
            HttpHandler.HandlePOST(context, out strParams);

            if (strParams is null)
                throw new ArgumentException();

            _user.Insert(new User(email: strParams["email"], password: strParams["password"]));
            //orm.ExecuteNonQuery<User>(sqlExp);
            context.Response.Redirect($@"http://localhost:8080/users/get_users/");
        }
        [HttpPOST("sing_up_user")]
        public bool Login(string email, string password)
        {
            
            var data = _user.FindByEmailAndPassword(email, password);
            return data != null;
            //orm.ExecuteNonQuery<User>(sqlExp);
        }
    }
}
