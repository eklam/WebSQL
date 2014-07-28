using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSQL.Infrastructure
{
    public class JSTreeAttribute
    {
        public string id { get; set; }
    }

    public class JSTree
    {
        public string data { get; set; }
        public JSTreeAttribute attr { get; set; }
        public List<JSTree> children { get; set; }
    }
}