using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HTTPResponse.Attributes;
using HTTPResponse.Repository;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using HTTPResponse.Models.UserModel;
using HTTPResponse.Context;


namespace HTTPResponse.Controllers
{
    [HttpController("users")]
    internal class UsersController
    {
        private static UserRepository ur = new UserRepository(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectORISDB;Integrated Security=True");

        public UsersController() { }

        [HttpGET("get_user")]
        public (byte[], WebHeaderCollection content_type) GetUserById (int id)
        {
            var user = ur.FindById(id);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");

            Context<User> context = new Context<User>("User", user);
            List<Context<User>> list = new List<Context<User>>();
            list.Add(context);

            string stringForResponse = "<b>{{ User.name }}</b>";

            var ins = new HTMLGeneratorClass.HTMLGenerator();
            stringForResponse = ins.GetHTML(stringForResponse, list);

            byte[] buffer = Encoding.UTF8.GetBytes(stringForResponse);
            return (buffer, headers);
        }

        [HttpGET("get_users")]
        public (byte[], WebHeaderCollection content_type) GetUsers()
        {

            List<User> users = new List<User>();
            users = ur.FindAll();

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Set("Content-Type", "text/css");
            headers.Add("Content-Type", "text/html");


            Context<List<User>> context = new Context<List<User>>("Users", users);
            List<Context<List<User>>> list = new List<Context<List<User>>>();
            list.Add(context);

            string stringForResponse = "<h3>{{ for user in Users }}";
            stringForResponse += "<p><b>{{ user.name }}</b></p>";
            stringForResponse += "{{ endfor }}</h3>";

            var ins = new HTMLGeneratorClass.HTMLGenerator();
            stringForResponse = ins.GetHTML(stringForResponse, list);

            byte[] buffer = Encoding.UTF8.GetBytes(stringForResponse);
            return (buffer, headers);
        }

        [HttpPOST("save_user")]
        public void SaveUser(int id, string name)
        {
            //string sqlExp = $"INSERT INTO [User] (name, surname, password) VALUES ('{name}', 'Anonimous', '{id}')";
            ur.Insert(new User(id, name));
            //orm.ExecuteNonQuery<User>(sqlExp);
        }
    }

}
