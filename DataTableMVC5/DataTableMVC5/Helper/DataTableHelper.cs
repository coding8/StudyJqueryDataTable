using MvcDataTable;
using System.Linq;
using System.Web.Mvc;

namespace MvcDataTableHelper
{
    public class DataTableHelper
    {
        /// <summary>返回JsonResult对象</summary>
        /// <typeparam name="T">实体或视图模型</typeparam>
        /// <param name="param">DataTable参数</param>
        /// <param name="query">传入查询参数</param>
        /// <returns>JsonResult</returns>
        public static JsonResult GetQuery<T>(DataTablesParam param, IQueryable<T> query)
        {
            //传进来的数据记录数(未筛选)
            var totalRecords = query.Count();

            //自定义搜索
            if (string.IsNullOrEmpty(param.sSearch))
            {
                //And
                if (param.Where.groupOp == "AND")
                    foreach (var rule in param.Where.rules)
                        query = query.Where<T>(
                                rule.field,
                                rule.data,
                                (WhereOperation)StringEnum.Parse(typeof(WhereOperation), rule.op)
                            );
                else
                {  //更新--2013.10.26
                    IQueryable<T> temp = null;
                    foreach (var rule in param.Where.rules)
                    {
                        var rule1 = rule;
                        var t = query.Where<T>(
                                rule1.field,
                                rule1.data,
                                (WhereOperation)StringEnum.Parse(typeof(WhereOperation), rule1.op)
                            );

                        if (temp == null)
                            temp = t;
                        else
                            //temp = temp.Union(t); // Union!!
                            temp = temp.Concat<T>(t);//使用union会导致查不出数据
                    }
                    query = temp.Distinct<T>();
                }
            }
            else//TODO: 全局搜索
            {

            }

            //sorting
            query = query.OrderBy<T>(param.SortColumn, param.SortOrder);

            //count
            var count = query.Count();

            //paging
            var data = query.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            var result = new
              {
                  sEcho = param.sEcho,
                  iTotalRecords = totalRecords,
                  iTotalDisplayRecords = count,
                  aaData = (from d in data
                            select d
                           ).ToArray()
              };

            //实例化JsonResult
            JsonResult jr = new JsonResult();
            jr.Data = result;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }
    }
}