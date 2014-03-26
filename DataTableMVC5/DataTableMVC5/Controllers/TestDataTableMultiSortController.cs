using DataTableMVC5.Models;
using DataTableMVC5.Models.Test;
using System.Linq;
using System.Web.Mvc;

namespace DataTableMVC5.Controllers
{
    public class TestDataTableMultiSortController : Controller
    {
        // 全局筛选遇到int字段无法在L2E中转换：LINQ to Entities 不识别方法“System.String ToString()”，因此该方法无法转换为存储表达式。
        // 多列排序似乎可用
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAjax(DataTablesParam param)
        {
            //string[] columnNames = { "EmployeeID", "Name", "Position" };
            string[] columnNames = { "Name", "Position" };
            //DataType[] types = { DataType.tInt, DataType.tString, DataType.tString };
            DataType[] types = { DataType.tString, DataType.tString };

            MyTableDbContext db = new MyTableDbContext();

            var query = db.Employee as IQueryable<Employee>;
            return DataTableHelper.GetQuery(param, columnNames, types, query);
        }
	}
}