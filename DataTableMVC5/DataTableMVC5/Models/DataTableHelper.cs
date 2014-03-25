using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataTableMVC5.Models
{
    public class DataTableHelper
    {
        public static JsonResult GetQuery<T>(DataTablesParam param, string[] columnNames, DataType[] types, IQueryable<T> query)
        {
            ////First we create an array with the dataBases column names used in the dataTable, in the same order we render then.
            //string[] columnNames = { "engine", "browser", "platform", "version", "grade" };
            //// Then an array of the same size as the previous one, where we specify, in the same order, the data type of each column
            //DataType[] types = { DataType.tString, DataType.tString, DataType.tString, DataType.tInt, DataType.tString };
            // set the linq data contex
            //DataTables.Models.DataClasses1DataContext db =
            //    new DataTables.Models.DataClasses1DataContext(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);

            //Get the total numer of records we can show on the table
            int totalRecords = query.Count();
            // Select the data we whant to present at the table, we can take any data we whant in any way, as long as it return an IQueryable
            var data = query;

            // Variable to store the records to display after the searches filters
            int totalRecordsDisplay;

            DataTableFilter filters = new DataTableFilter();

            // Call of our filter function
            data = filters.FilterPagingSortingSearch(param, data, out totalRecordsDisplay, columnNames, types) as IQueryable<T>;
            var listData = data.ToList();

            var result = new
            {
                iTotalRecords=totalRecords,
                iTotalDisplayRecords=totalRecordsDisplay,
                sEcho=param.sEcho,
                aaData=(from d in listData select d).ToArray()
            };
            //实例化JsonResult
            JsonResult jr = new JsonResult();
            jr.Data = result;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;

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