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

    public class MyTableDbContext : DbContext
    {
        public DbSet<DataTablesWithJson> DataTablesWithJson { get; set; }
    }
}