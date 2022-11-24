using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPResponse
{
    class ServerSettings
    {
        public int Port { get; set; } = 7070;
        public string Path { get; set; } = "./site";
        public string SqlConnection { get; set; } = "@\"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ProjectORISDB;Integrated Security=True\"";
    }
}
