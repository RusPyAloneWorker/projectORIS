using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace HTTPResponse.DBReader
{
    public class DBReaderClass
    {
        public static DataTable GetData<T>(string table)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectORISDB;Integrated Security=True";
            DataTable dt = new DataTable();

            string sqlExpression = $"SELECT * FROM [{table}]";
            SqlConnection connection = new SqlConnection(connectionString);
            using (var daSelectDeviceProperties = new SqlDataAdapter(sqlExpression, connection))
            {
                // no need to open/close the connection with DataAdapter.Fill
                daSelectDeviceProperties.Fill(dt);
            }
            connection.Close();
            return dt;
        }
        public static void PostData<T>(string table, string expr)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ProjectORISDB;Integrated Security=True";

            string sqlExpression = expr;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine("Добавлено объектов: {0}", number);
            }
        }
    }
}
