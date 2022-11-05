using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HTTPResponse.Attributes;
using HTTPResponse.MyORM;
using System.Data.SqlClient;
using System.IO;

namespace HTTPResponse.Controllers
{
    [HttpController("users")]
    internal class Users
    {  
        private static ORM orm = new ORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectORISDB;Integrated Security=True");
        [HttpGET("get_user")]
        public User GetUserById (int id)
        {
            var users = orm.Select<User>();
            return users.FirstOrDefault(t => t.user_id == id);
        }
        [HttpGET("get_users")]
        public List<User> GetUsers()
        {
            List<User> result = new List<User>();
            result = orm.Select<User>();
            return result;
        }
        [HttpPOST("save_user")]
        public void SaveUser(int id, string name)
        {
            //string sqlExp = $"INSERT INTO [User] (name, surname, password) VALUES ('{name}', 'Anonimous', '{id}')";
            orm.Insert<User>(new User(id, name));
            //orm.ExecuteNonQuery<User>(sqlExp);
        }
    }
    internal class User
    {
        [Key]
        public int user_id { get; private set; }
        public string name { get; private set; }
        public string surname { get; private set; }
        public string password { get; private set; }
        public User() { }
        public User(int id, string name = "Аноним", string surname = "Анонимыч")
        {
            user_id = id;
            this.name = name;
            this.surname = surname;
            password = "1111";
        }
        public User(int id, string name, string surname, string password)
        {
            user_id = id;
            this.name = name;
            this.surname = surname;
            this.password = password;
        }
    }
}
