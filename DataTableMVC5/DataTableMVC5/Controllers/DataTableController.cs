using DataTableMVC5.Models;
using MvcDataTable;
using MvcDataTableHelper;
using System.Linq;
using System.Web.Mvc;

namespace DataTableMVC5.Controllers
{
    public class DataTableController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAjax(DataTablesParam param)
        {
            MyTableDbContext db = new MyTableDbContext();

            var query = db.Employee as IQueryable<Employee>;
            return DataTableHelper.GetQuery(param, query);
        }

        //TODO:导出到Excel
	}
}