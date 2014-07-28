using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.BusinessObjects
{
    public class Databases
    {
        public List<Tables> Tables { get; set; }
        public string DatabaseName { get; set; }
        public Databases()
        {
            Tables = new List<Tables>();
        }
    }

    public class Tables
    {
        public string ObjectId { get; set; }
        public string Name { get; set; }
        public List<Columns> Columns { get; set; }
        public Tables()
        {
            Columns = new List<Columns>();
        }
    }

    public class Columns
    {
        public string ObjectId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public class ServerInfo
    {
        public List<Databases> DatabaseList { get; set; }
        public ServerInfo()
        {
            DatabaseList = new List<Databases>();
        }
    }
}
