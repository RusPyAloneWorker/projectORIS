using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTTPResponse.Attributes;
using HTTPResponse.Repository;

namespace HTTPResponse.Models.UserModel
{
    internal class User : UserRepository
    {
        [Key]
        public int user_id { get; private set; }
        public string name { get; private set; }
        public string surname { get; private set; }
        public string password { get; private set; }
        public User() { }
        public User(int id, string name = "Аноним", string surname = "Анонимыч")
        {
            this.name = name;
            this.surname = surname;
            password = Convert.ToString(id);
        }
        public User(int id, string name, string surname, string password)
        {
            this.user_id = id;
            this.name = name;
            this.surname = surname;
            this.password = password;
        }
    }
}
