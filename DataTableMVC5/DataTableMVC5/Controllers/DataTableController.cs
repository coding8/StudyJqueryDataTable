using DataTableMVC5.Models;
using MvcDataTable;
using MvcDataTableHelper;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DataTableMVC5.Controllers
{
    public class DataTableController : Controller
    {
        MyTableDbContext db = new MyTableDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public string GetAjax(DataTablesParam param)
        {
            var query = db.Employee as IQueryable<Employee>;
            return DataTableHelper.GetQuery(param, query);
        }

        // 导出到Excel
        public ActionResult ExportToExcel(DataTablesParam pp, string btName)
        {
            //构造一个GridSettings实例接收筛选后的数据
            DataTablesParam param = new DataTablesParam();
            param.SortColumn = "Name";
            param.SortOrder = "asc";
            param.bFilter = Convert.ToBoolean(Request.QueryString["IsFilter"]);//true or false

            //筛选条件
            param.filters = Request.QueryString["filters"];//写入模型.若没输入查询条件则filters为空
            param.Where = MvcDataTable.Filter.Create(param.filters, "", "", "");

            var query = db.Employee as IQueryable<Employee>;
            var filteredData = DataTableHelper.GetFilteredData(param, query);

            //获得数据后按需求输出相关字段
            var myGrid = new System.Web.UI.WebControls.GridView();
            myGrid.DataSource = (from p in filteredData
                                 select new
                                 {
                                     姓名 = p.Name,
                                     Position = p.Position,
                                     Birthday = p.Birthday.ToShortDateString()
                                 }).ToList();
            myGrid.DataBind();
            ExportToExcel(myGrid, "姓名");

            return Content("");
        }

        /// <summary>导出到Excel（use WebControl）</summary>
        /// <param name="ctrl">包含数据的控件</param>
        /// <param name="fileName">文件名</param>
        public static void ExportToExcel(WebControl ctrl, string fileName)
        {
            string outputFileName = null;
            string browser = System.Web.HttpContext.Current.Request.UserAgent.ToUpper();

            //消除文件名乱码。如果是IE则编码文件名，如果是FF则在文件名前后加双引号。
            if (browser.Contains("MS") == true && browser.Contains("IE") == true)
                outputFileName = HttpUtility.UrlEncode(fileName);  //%e5%90%8d%e5%8d%95
            else if (browser.Contains("FIREFOX") == true)
                outputFileName = "\"" + fileName + ".xls\"";  //"名单.xls" 
            else
                outputFileName = HttpUtility.UrlEncode(fileName);

            HttpResponse Response = System.Web.HttpContext.Current.Response;

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=" + outputFileName + ".xls");
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.Charset = "gb2312";
            Response.ContentType = "application/ms-excel";

            StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            ctrl.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        //CRUD
        public ActionResult CRUD()
        {
            IQueryable<Employee> query = db.Employee;
            return View(query);
        }
        //public int AddData(string Name, string Position, DateTime Birthday, int CompanyID)
        public ActionResult AddData(Employee emp)
        {
            if (db.Employee.Any(c => c.Name.ToLower().Equals(emp.Name.ToLower())))
            {
                Response.Write("Employee with the name '" + emp.Name + "' already exists");
                Response.StatusCode = 404;
                Response.End();
                //return -1;
                return HttpNotFound();
            }
            
            db.Employee.Add(emp);
            db.SaveChanges();
            //return emp.EmployeeID;
            return RedirectToAction("CRUD");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}