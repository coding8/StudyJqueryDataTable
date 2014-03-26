using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace DataTableMVC5.Models.Test
{
    [ModelBinder(typeof(DataTablesModelBinding))]
    [Serializable]
    public class DataTablesParam
    {
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public int iColumns { get; set; }
        public string sSearch { get; set; }
        public bool bEscapeRegex { get; set; }
        public int iSortingCols { get; set; }
        public string sEcho { get; set; }
        public List<bool> bSortable { get; set; }
        public List<bool> bSearchable { get; set; }
        public List<string> sSearchColumns { get; set; }
        public List<int> iSortCol { get; set; }
        public List<string> sSortDir { get; set; }
        public List<bool> bEscapeRegexColumns { get; set; }

        public DataTablesParam()
        {
            bSortable = new List<bool>();
            bSearchable = new List<bool>();
            sSearchColumns = new List<string>();
            iSortCol = new List<int>();
            sSortDir = new List<string>();
            bEscapeRegexColumns = new List<bool>();
        }
    }
    public class DataTablesModelBinding : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            DataTablesParam obj = new DataTablesParam();
            var request = controllerContext.HttpContext.Request.Params;
            // First we take the single params
            obj.iDisplayStart = Convert.ToInt32(request["iDisplayStart"]);
            obj.iDisplayLength = Convert.ToInt32(request["iDisplayLength"]);
            obj.iColumns = Convert.ToInt32(request["iColumns"]);
            obj.sSearch = request["sSearch"];
            obj.bEscapeRegex = Convert.ToBoolean(request["bEscapeRegex"]);
            obj.iSortingCols = Convert.ToInt32(request["iSortingCols"]);
            obj.sEcho = request["sEcho"];
            // Now we take the params in the format iSortCol_(int) and save then in lists
            for (int i = 0; i < obj.iColumns; i++)
            {
                obj.bSortable.Add(Convert.ToBoolean(request["bSortable_" + i]));
                obj.bSearchable.Add(Convert.ToBoolean(request["bSearchable_" + i]));
                obj.sSearchColumns.Add(request["sSearch_" + i]);
                obj.bEscapeRegexColumns.Add(Convert.ToBoolean(request["bEscapeRegex_" + i]));
                obj.iSortCol.Add(Convert.ToInt32(request["iSortCol_" + i]));
                obj.sSortDir.Add(request["sSortDir_" + i]);
            }
            return obj;
        }
    }

    public enum DataType
    {
        tInt,
        tString,
        tnone
    }

    public class DataTableFilter
    {
        /// <summary>
        /// Receive an IQueryable, apply all the filters from DataTablesParam (paging, sorting and search)
        /// and return it in another IQueryable
        /// </summary>
        /// <param name="DTParams">The parameters from DataTable</param>        
        /// <param name="data">The data we whant to appli the filters</param>
        /// <param name="totalRecordsDisplay">The number of records we can display, after the search filter</param>
        /// <param name="columnNames">Array with the names of the data base columns we are going to render, in the same order we render then on the client</param>
        /// <param name="types">Array of the data type of the columns, one for each column in columnNames, in the same order</param>
        /// <returns></returns>
        public IQueryable FilterPagingSortingSearch(DataTablesParam DTParams, IQueryable data, out int totalRecordsDisplay,
            string[] columnNames, DataType[] types)
        {
            // If the search field is not empty
            if (!String.IsNullOrEmpty(DTParams.sSearch))
            {
                // We build the query to pass to Linq. The final query should look something like: 
                //    "(engine.Contains("pepe") or grade.Contains("pepe"))"
                string searchString = "";
                bool first = true;
                for (int i = 0; i < DTParams.iColumns; i++)
                {
                    if (DTParams.bSearchable[i]) // If the column is marked as searchable
                    {
                        // We get the column name
                        string columnName = columnNames[i];

                        if (!first)
                            searchString += " or ";
                        else
                            first = false;

                        // If the column it's a integer, we only check if it starts with the search parameter send
                        if (types[i] == DataType.tInt)
                        {
                            searchString += columnName + ".ToString().StartsWith(\"" + DTParams.sSearch + "\")";// LINQ to Entities 不识别方法“System.String ToString()”，因此该方法无法转换为存储表达式。
                            //searchString += columnName + "" + ".StartsWith(\"" + DTParams.sSearch + "\")";错误 int不包含StartsWith
                            //searchString += columnName + ".StartsWith(\"" + DTParams.sSearch + "\")"; 错误 int不包含StartsWith
                            //searchString += columnName + ".Equals(\"" + DTParams.sSearch + "\")";// 其他信息: DbComparisonExpression 需要具有可比较类型的参数。

                        }
                        // If it's a string we search if it contain the search parameter.
                        else
                        {
                            searchString += columnName + ".Contains(\"" + DTParams.sSearch + "\")";
                        }
                    }
                }
                // we call the linq dynamic function where with the string containing the query
                data = data.Where(searchString);
            }

            // Now we build the search query, should look something like:
            // "(engine desc, browser asc,grade desc)"
            string sortString = "";
            for (int i = 0; i < DTParams.iSortingCols; i++) //iSortingCols Tell us the number of columns to sort
            {
                // We get the column name
                int columnNumber = DTParams.iSortCol[i];
                string columnName = columnNames[columnNumber];
                // Get the direction to sort the column
                string sortDir = DTParams.sSortDir[i];
                if (i != 0)
                    sortString += ", ";
                sortString += columnName + " " + sortDir;
            }

            // We get the number of records to display after de search
            //错误：需要具有可比较类型的参数
            totalRecordsDisplay = data.Count(); //其他信息: LINQ to Entities 不识别方法“System.String ToString()”，因此该方法无法转换为存储表达式。

            // call the linq dynamic OrderBy with our query
            data = data.OrderBy(sortString);
            // paging function
            data = data.Skip(DTParams.iDisplayStart).Take(DTParams.iDisplayLength);

            return data;
        }
    }

    public class DataTableHelper
    {
        public static JsonResult GetQuery<T>(DataTablesParam param, string[] columnNames, DataType[] types, IQueryable<T> query)
        {
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
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecordsDisplay,
                sEcho = param.sEcho,
                aaData = (from d in listData select d).ToArray()
            };
            //实例化JsonResult
            JsonResult jr = new JsonResult();
            jr.Data = result;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }
    }
}