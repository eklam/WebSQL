using BusinessLayer.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSQL.Infrastructure
{
    public class JSONResponse
    {
        public List<JSTree> JSTree { get; set; }
        public ServerInfo ServerInfo { get; set; }

        public JSONResponse()
        {
            JSTree = new List<JSTree>();
            ServerInfo = new ServerInfo();
        }
    }
}