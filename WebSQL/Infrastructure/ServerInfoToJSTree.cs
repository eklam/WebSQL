using BusinessLayer.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSQL.Infrastructure
{
    public static class ServerInfoToJSTree
    {

        public static List<JSTree> ToTree(ServerInfo serverInfo)
        {
            List<JSTree> output = new List<JSTree>();
            foreach (var db in serverInfo.DatabaseList)
            {
                JSTree jsDB = new JSTree();
                jsDB.data = db.DatabaseName;
                foreach (var tbl in db.Tables)
                {
                    JSTree jsTbl = new JSTree
                    {
                        data = tbl.Name
                    };

                    if (jsDB.children == null)
                        jsDB.children = new List<JSTree>();
                    jsDB.children.Add(jsTbl);
                }
                output.Add(jsDB);
            }
            return output;
        }
    }
}