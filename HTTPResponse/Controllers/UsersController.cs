using System;
using System.Collections.Generic;
using System.Linq;
using HTTPResponse.Attributes;
using System.Net;
using HTTPResponse.Models.UserModel;
using HTTPResponse.Context;
using RazorEngine;
using RazorEngine.Templating;


namespace HTTPResponse.Controllers
{
    [HttpController("users")]
    internal class UsersController
    {
        private static User _user = new User();
        public UsersController() { }

        [HttpGET("get_user")]
        public (byte[], WebHeaderCollection content_type) GetUserById (int id)
        {
            var user = _user.FindById(id);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");

            Context<User> context = new Context<User>("User", user);

            string stringForResponse = "<b>{{ User.name }}</b>";

            stringForResponse = Engine.Razor.RunCompile(stringForResponse, "templateKey", null, context);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(stringForResponse);
            return (buffer, headers);
        }

        [HttpGET("get_users")]
        public (byte[], WebHeaderCollection content_type) GetUsers()
        {

            List<User> users = new List<User>();
            users = _user.FindAll();

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");


            Context<List<User>> context = new Context<List<User>>("Users", users);

            string stringForResponse = "<h3>{{ for user in Users }}";
            stringForResponse += "<p><b>{{ user.name }}</b></p>";
            stringForResponse += "{{ endfor }}</h3>";

            stringForResponse = Engine.Razor.RunCompile(stringForResponse, "templateKey", null, context);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(stringForResponse);
            return (buffer, headers);
        }

        [HttpPOST("save_user")]
        public void SaveUser(int id, string name)
        {
            //string sqlExp = $"INSERT INTO [User] (name, surname, password) VALUES ('{name}', 'Anonimous', '{id}')";
            _user.Insert(new User(id, name));
            //orm.ExecuteNonQuery<User>(sqlExp);
        }
    }

}
