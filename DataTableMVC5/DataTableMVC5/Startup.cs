using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DataTableMVC5.Startup))]
namespace DataTableMVC5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
