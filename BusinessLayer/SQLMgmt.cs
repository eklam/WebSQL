using BusinessLayer.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.Data;

namespace BusinessLayer
{
    public class SQLMgmt
    {

        public ServerInfo GetServerInfo(string serverName, string userName, string password)
        {
            ServerInfo output = new ServerInfo();
            
            string databaseName = "master";
            ConnectionStringProcessor cnProcessor = new ConnectionStringProcessor();
            string connString = cnProcessor.GenerateConnectionString(serverName, userName, password, databaseName);
            SqlConnection conn = new SqlConnection(connString);
            Server sqlServer = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection(conn));
            var databases = sqlServer.Databases;
            foreach (Database db in databases)
            {
                try
                {
                    if (!db.IsAccessible)
                        continue;
                }
                catch (Exception ex)
                {
                }
                var database = new Databases { DatabaseName = db.Name };
                foreach (Table tbl in db.Tables)
                {
                    Tables tblModel = new Tables { Name = tbl.Schema + "." + tbl.Name, ObjectId = tbl.ID.ToString() };
                    //foreach (Column col in tbl.Columns)
                    //{
                    //    Columns colModel = new Columns { Name = col.Name, Type = col.DataType.Name, ObjectId = col.ID.ToString() };
                    //    tblModel.Columns.Add(colModel);
                    //}

                    database.Tables.Add(tblModel);
                }
                output.DatabaseList.Add(database);
            }
            return output;
        }

        public class SQLAsyncReturn
        {
            public DataTable DataTable { get; set;  }
            public bool IsFinished { get; set; }
            public SQLAsyncReturn(DataTable dataTable, bool isFinished)
            {
                DataTable = dataTable;
                IsFinished = IsFinished;
            }
        }

        public void ExecuteSQLAsync(string server, string user, string password, string database, string sql, Action<SQLAsyncReturn> asyncReturn)
        {
            ConnectionStringProcessor cnProcessor = new ConnectionStringProcessor();
            string connString = cnProcessor.GenerateConnectionString(server, user, password, database);
            DataTable output = new DataTable();
            //sql = AppendTopToSelect(sql);
            int numRows = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandTimeout = 2000;
                SqlDataReader reader = cmd.ExecuteReader();
                
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    DataColumn col = new DataColumn(reader.GetName(i));
                    output.Columns.Add(col);
                }

                while (reader.Read())
                {
                    numRows++;
                    DataRow row = output.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var c = reader[i];
                        row[i] = c.ToString();
                    }
                    output.Rows.Add(row);
                    if (numRows % 1000 == 0)
                    {
                        asyncReturn(new SQLAsyncReturn(output, false));
                        output.Rows.Clear();
                    }
                }
                if (output.Rows.Count > 0)
                    asyncReturn(new SQLAsyncReturn(output, true));
                else
                    asyncReturn(new SQLAsyncReturn(new DataTable(), true));
            }
            
        }

        public DataTable ExecuteSQL(string server, string user, string password, string database, string sql)
        {
            ConnectionStringProcessor cnProcessor = new ConnectionStringProcessor();
            string connString = cnProcessor.GenerateConnectionString(server, user, password, database);
            DataTable output = new DataTable();
            sql = AppendTopToSelect(sql);
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                //using (var adapter = new SqlDataAdapter(sql, conn))
                //{
                //    adapter.Fill(output);
                //}
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                for (int i = 0; i < reader.FieldCount; i++ )
                {
                    DataColumn col = new DataColumn(reader.GetName(i));
                    output.Columns.Add(col);
                }

                while (reader.Read())
                {
                    DataRow row = output.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var c = reader[i];
                        row[i] = c.ToString();
                    }
                    output.Rows.Add(row);

                }
            }
            return output;
        }

        private string AppendTopToSelect(string sql)
        {
            var lastIndexOfSelect = sql.ToLower().LastIndexOf("select");
            if (lastIndexOfSelect < 0)
                return sql;
            return sql.Insert(lastIndexOfSelect + 7, "top 1000 ");
        }

        public List<Columns> GetColumnsForTable(string serverName, string userName, string password, string databaseName, string fulltableName)
        {
            List<Columns> output = new List<Columns>();
            ConnectionStringProcessor cnProcessor = new ConnectionStringProcessor();
            string connString = cnProcessor.GenerateConnectionString(serverName, userName, password, databaseName);
            SqlConnection conn = new SqlConnection(connString);
            Server sqlServer = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection(conn));
            var database = sqlServer.Databases[databaseName];
            if (database == null)
                return output;

            string tableName = fulltableName.Substring(fulltableName.LastIndexOf('.') + 1);
            string schemaName = fulltableName.Substring(0, fulltableName.LastIndexOf('.'));
            var table = database.Tables[tableName, schemaName];
            if (table == null)
                return output;

            foreach (Column col in table.Columns)
            {
                Columns colModel = new Columns { Name = col.Name, Type = col.DataType.Name, ObjectId = col.ID.ToString() };
                output.Add(colModel);
            }
            return output;
        }
    }
}
