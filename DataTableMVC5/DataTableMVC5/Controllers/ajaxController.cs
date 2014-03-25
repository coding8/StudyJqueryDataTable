using DataTableMVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataTableMVC5.Controllers
{
    public class ajaxController : Controller
    {
        //
        // GET: /ajax/
        public ActionResult Index()
        {
            return View();
        }
        //public ContentResult GetAjax(DataTablesParam dataTableParam)
        //{
        //    AjaxBLL ajax = new AjaxBLL();
        //    string jsonResult = ajax.ListAjax(dataTableParam);
        //    return Content(jsonResult, "text/html");     // Maybe the content type should be application/json       
        //}

        public ActionResult GetAjax(DataTablesParam param)
        {
            //string[] columnNames = { "EmployeeID", "Name", "Position" };
            string[] columnNames = { "Name", "Position" };
            //DataType[] types = { DataType.tInt, DataType.tString, DataType.tString };
            DataType[] types = {  DataType.tString, DataType.tString };

            MyTableDbContext db = new MyTableDbContext();

            var query = db.Employee as IQueryable<Employee>;
            return DataTableHelper.GetQuery(param,columnNames,types,query);

            //int totalRecords = db.Employee.Count();
            ////var data = db.Employee.AsQueryable();
            //var data = db.Employee as IQueryable<Employee>;
            //int totalRecordsDisplay;

            //DataTableFilter filters = new DataTableFilter();

            //data = filters.FilterPagingSortingSearch(param, data, out totalRecordsDisplay, columnNames, types)
            //    as IQueryable<Employee>;
            //var listData = data.ToList();

            //// Now we build the Json response
            //string jsonResponse;
            //jsonResponse = "{\"iTotalRecords\":" + totalRecords + ",\"iTotalDisplayRecords\":" + totalRecordsDisplay + ",\"sEcho\":" + param.sEcho +
            //    ",\"aaData\":[";

            //// Build of the data for each row returned
            //for (int i = 0; i < listData.Count; ++i)
            //{
            //    var ajx = listData[i];
            //    if (i > 0)
            //        jsonResponse += ",";
            //    jsonResponse += "[\"" + ajx.engine + "\",\"" + ajx.browser + "\",\"" + ajx.platform + "\",\"" + ajx.version + "\",\"" + ajx.grade + "\"]";
            //}
            //jsonResponse += "]}";
            //return jsonResponse;
        }
	}
}