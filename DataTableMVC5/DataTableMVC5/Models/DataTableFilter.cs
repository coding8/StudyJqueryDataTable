using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;
using System.Data.Entity.SqlServer;
namespace DataTableMVC5.Models
{
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
                            //searchString += columnName + ".ToString().StartsWith(\"" + DTParams.sSearch + "\")";// LINQ to Entities 不识别方法“System.String ToString()”，因此该方法无法转换为存储表达式。
                            //searchString += columnName + "" + ".StartsWith(\"" + DTParams.sSearch + "\")";错误 int不包含StartsWith
                            //searchString += columnName + ".StartsWith(\"" + DTParams.sSearch + "\")"; 错误 int不包含StartsWith
                            searchString += columnName + ".Equals(\"" + DTParams.sSearch + "\")";// 其他信息: DbComparisonExpression 需要具有可比较类型的参数。
                          
                        }
                        // If it's a string we search if it contain the search parameter.
                        else
                        {
                            //searchString += columnName + ".ToString().Contains(\"" + DTParams.sSearch + "\")";
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
}