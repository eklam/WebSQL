using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSQL.Infrastructure
{
    public static class MySession
    {
        public static SessionInfo MySessionInfo
        {
            get
            {
                return HttpContext.Current.Session["MySessionInfo"] as SessionInfo;
            }
            set
            {
                HttpContext.Current.Session["MySessionInfo"] = value;
            }
        }
    }
    
    public class SessionInfo
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}