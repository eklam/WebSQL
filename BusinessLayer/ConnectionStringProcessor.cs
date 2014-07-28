using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ConnectionStringProcessor
    {
        public string GenerateConnectionString(string server, string user, string password, string database)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.UserID = user;
            builder.Password = password;
            builder.InitialCatalog = database;
            builder.ConnectTimeout = 500;
            builder.Pooling = false;

            return builder.ConnectionString;
        }
    }
}
