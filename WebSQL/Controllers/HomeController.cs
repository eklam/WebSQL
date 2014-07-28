using BusinessLayer;
using BusinessLayer.BusinessObjects;
using Microsoft.AspNet.SignalR;
using MyJQGrid;
using PerpetuumSoft.Knockout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSQL.Infrastructure;
using WebSQL.Models;
using WebSQL.SignalR;

namespace WebSQL.Controllers
{
    public partial class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            return RedirectToAction("Start/");   
        }

        public virtual ActionResult Start()
        {
            if (MySession.MySessionInfo != null)
                return View("Index", new IndexModel(true));
            else
                return View("Index", new IndexModel(false));
        }

        public virtual JsonResult LoginToServerAuth()
        {
            SQLMgmt sqlMgmt = new SQLMgmt();
            try
            {
                ServerInfo serverInfo = sqlMgmt.GetServerInfo(MySession.MySessionInfo.ServerName, MySession.MySessionInfo.UserName, MySession.MySessionInfo.Password);
                var tree = ServerInfoToJSTree.ToTree(serverInfo);
                JSONResponse response = new JSONResponse
                {
                    ServerInfo = serverInfo,
                    JSTree = tree
                };
                StandardBooleanSuccess sbs = new StandardBooleanSuccess(true) { AdditionalData = response };
                return Json(sbs);
            }
            catch (Exception ex)
            {
                MySession.MySessionInfo = null;
                return Json(new StandardBooleanSuccess("Could not log into server.  Check credentials and try again."));
            }
            
        }

        public virtual JsonResult LogOff()
        {
            Session.Clear();
            return Json(new StandardBooleanSuccess(true));
        }

        public virtual JsonResult FetchColumns(string database, string table)
        {
            SQLMgmt sqlMgmt = new SQLMgmt();
            if (MySession.MySessionInfo == null)
                return Json(new List<Columns>());
            List<Columns> columns = sqlMgmt.GetColumnsForTable(MySession.MySessionInfo.ServerName, MySession.MySessionInfo.UserName,
                MySession.MySessionInfo.Password, database, table);
            return Json(columns);
        }

        public virtual JsonResult LoginToServer(string serverName, string userName, string password)
        {
            SQLMgmt sqlMgmt = new SQLMgmt();
            try
            {
                
                ServerInfo serverInfo = sqlMgmt.GetServerInfo(serverName, userName, password);
                var tree = ServerInfoToJSTree.ToTree(serverInfo);
                MySession.MySessionInfo = new SessionInfo { ServerName = serverName, Password = password, UserName = userName };
                JSONResponse response = new JSONResponse
                {
                    ServerInfo = serverInfo,
                    JSTree = tree
                };
                StandardBooleanSuccess sbs = new StandardBooleanSuccess(true) { AdditionalData = response };
                
                return Json(sbs);
            }
            catch (Exception ex)
            {
                return Json(new StandardBooleanSuccess("Could not log into server.  Check credentials and try again."));
            }
        }

        public virtual JsonResult RunSQL2(string sql, string database)
        {
            SQLMgmt sqlMgmt = new SQLMgmt();
            

            sqlMgmt.ExecuteSQLAsync(MySession.MySessionInfo.ServerName, MySession.MySessionInfo.UserName, MySession.MySessionInfo.Password, database, sql,
                ret => 
                {
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyConnection>();
                    JQGridColParams colParams = new JQGridColParams();
                    foreach (DataColumn column in ret.DataTable.Columns)
                    {
                        colParams.AddCol(column.ColumnName, 100);
                    }
                    var myData = ConvertDT(ret.DataTable);
                    JQGrid<object> output = new JQGrid<object>(myData, colParams);
                    output.Width = 1000;
                    output.Height = 400;
                    output.IsLastRecordSet = ret.IsFinished;
                    var jsonResult = Json(output);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    

                    hubContext.Clients.All.RefreshResults(jsonResult);
                });
            
            
           // hubContext.Clients.Group("coh_" + cohortId.ToString()).helloworld(message);
            return Json(new StandardBooleanSuccess(true));
            
        }

        

        /// this method converts the data table to list object
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<object> ConvertDT(DataTable dt)
        {
            List<object> output = new List<object>();
            foreach (DataRow dRow in dt.Rows)
            {
                Hashtable hashtable = new Hashtable();
                foreach (DataColumn column in dt.Columns)
                {
                    //if (column.DataType == typeof(int))
                    //{
                    //    hashtable.Add(column.ColumnName, int.Parse(dRow[column.ColumnName].ToString()));
                    //}
                    //else
                    //{
                        hashtable.Add(column.ColumnName, dRow[column.ColumnName].ToString());
                    //}
                }
                output.Add(hashtable);
            }
            return output;

        }
    }
}
