using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSQL.Models
{
    public class IndexModel
    {

        public bool IsAuthenticated { get; set; }

        public IndexModel(bool isAuthenticated)
        {
            IsAuthenticated = isAuthenticated;
        }
    }
}