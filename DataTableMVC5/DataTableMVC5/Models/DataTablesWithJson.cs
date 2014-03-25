using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DataTableMVC5.Models
{
    public class DataTablesWithJson
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int CompanyID { get; set; }
        public DateTime Birthday { get; set; }
    }
    public class Company
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
    }

    public class MyTableDbContext : DbContext
    {
        public DbSet<DataTablesWithJson> DataTablesWithJson { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Company> Company { get; set; }
    }
}