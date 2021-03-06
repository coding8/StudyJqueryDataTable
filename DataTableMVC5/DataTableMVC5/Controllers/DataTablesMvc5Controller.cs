﻿using DataTableMVC5.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
//
namespace DataTableMVC5.Controllers
{
    public class DataTablesMvc5Controller : Controller
    {
        private MyTableDbContext db = new MyTableDbContext();

        public class jQueryDataTableParamModel
        {
            /// 
            /// Request sequence number sent by DataTable,
            /// same value must be returned in response
            ///        
            public string sEcho { get; set; }

            /// 
            /// Text used for filtering
            /// 
            public string sSearch { get; set; }

            /// 
            /// Number of records that should be shown in table
            /// 
            public int iDisplayLength { get; set; }

            /// 
            /// First record that should be shown(used for paging)
            /// 
            public int iDisplayStart { get; set; }

            /// 
            /// Number of columns in table
            /// 
            public int iColumns { get; set; }

            /// 
            /// Number of columns that are used in sorting
            /// 
            public int iSortingCols { get; set; }

            /// 
            /// Comma separated list of column names
            /// 
            public string sColumns { get; set; }
        }

        public ActionResult AjaxHand(
            jQueryDataTableParamModel param
        )
        {
            //return Json(new
            //{
            //    sEcho = param.sEcho,
            //    iTotalRecords = 97,
            //    iTotalDisplayRecords = 3,
            //    aaData = new List<string[]>() {
            //        new string[] {"1", "Microsoft"},
            //        new string[] {"2", "Google"},
            //        new string[] {"3", "Gowi"}
            //        }
            //},
            //JsonRequestBehavior.AllowGet);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = db.DataTablesWithJson.Count(),
                iTotalDisplayRecords = db.DataTablesWithJson.Count(),
                aaData = db.DataTablesWithJson.ToList()
            },
                JsonRequestBehavior.AllowGet
            );
        }
        // GET: /DataTablesMvc5/
        public ActionResult Index()
        {
            return View(db.DataTablesWithJson.ToList());
        }

        public ActionResult Refresh()
        {
            return View();
        }

        public ActionResult MasterDetailsAjaxHandler(
             jQueryDataTableParamModel param, int? EmployeeID, string Name)
        {

            var employees = db.Employee;

            //"Business logic" method that filters employees by the employer id and name
            //自定义筛选
            var companyEmployees = (from e in employees
                                    where (EmployeeID == null || e.EmployeeID == EmployeeID)
                                    &&
                                    (string.IsNullOrEmpty(Name) || e.Name == Name)
                                    select e).ToList();

            //UI processing logic that filter company employees by name and paginates them
            //全局搜索（在以上自定义筛选的基础上还可以筛选）【可选项】
            var filteredEmployees = (from e in companyEmployees
                                     where (
                                     param.sSearch == null
                                     ||
                                     e.Name.ToLower().Contains(param.sSearch.ToLower())
                                     ||
                                     e.EmployeeID.ToString().Contains(param.sSearch) //int转为string
                                     )
                                     select e).ToList();

            var result = from emp in filteredEmployees.Skip(
                         param.iDisplayStart).Take(param.iDisplayLength)
                         select new[] { 
                             //Convert.ToString(emp.EmployeeID), 
                             emp.EmployeeID.ToString(),
                             emp.Name,
                             emp.Position ,
                             emp.Birthday.ToString("yyyy-mm-dd") 
                         };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = companyEmployees.Count,
                iTotalDisplayRecords = filteredEmployees.Count,
                aaData = result
            },
            JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DataTablesMvc4/Create

        [HttpPost]
        public ActionResult Create(DataTablesWithJson datatableswithjson)
        {
            if (ModelState.IsValid)
            {
                db.DataTablesWithJson.Add(datatableswithjson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(datatableswithjson);
        }

        public ActionResult Details(int id = 0)
        {
            DataTablesWithJson datatableswithjson = db.DataTablesWithJson.Find(id);
            if (datatableswithjson == null)
            {
                return HttpNotFound();
            }
            return View(datatableswithjson);
        }

        // GET: /DataTablesMvc4/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DataTablesWithJson datatableswithjson = db.DataTablesWithJson.Find(id);
            if (datatableswithjson == null)
            {
                return HttpNotFound();
            }
            return View(datatableswithjson);
        }

        //
        // POST: /DataTablesMvc4/Edit/5

        [HttpPost]
        public ActionResult Edit(DataTablesWithJson datatableswithjson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(datatableswithjson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(datatableswithjson);
        }

        //
        // GET: /DataTablesMvc4/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DataTablesWithJson datatableswithjson = db.DataTablesWithJson.Find(id);
            if (datatableswithjson == null)
            {
                return HttpNotFound();
            }
            return View(datatableswithjson);
        }

        //
        // POST: /DataTablesMvc4/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DataTablesWithJson datatableswithjson = db.DataTablesWithJson.Find(id);
            db.DataTablesWithJson.Remove(datatableswithjson);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}