using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HTTPResponse.Attributes;
using HTTPResponse.DBReader;
using System.Data.SqlClient;

namespace HTTPResponse.Controllers
{
    [HttpController("users")]
    internal class Users
    {
        [HttpGET("get user")]
        public User GetUser (int id)
        {
            var dataTable = DBReaderClass.GetData<User>("User");
            var users = DatatableToListConverter.ConvertDataTable<User>(dataTable);
            return users.FirstOrDefault(t => t.user_id == id);
        }
        [HttpPOST("make user")]
        public void MakeUser(int id, string name)
        {
            string sqlExp = $"INSERT INTO [User] (name, surname, password) VALUES ('{name}', 'Anonimous', '{id}')";
            DBReaderClass.PostData<User>("User", sqlExp);
        }
    }
    [HttpController("user")]
    internal class User
    {
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
