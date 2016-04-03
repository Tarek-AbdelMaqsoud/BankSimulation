using Microsoft.AspNet.SignalR;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BankSimulation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string connString = ConfigurationManager.ConnectionStrings["BankSimulationContextDatabaseConnectionName"].ConnectionString;
       
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiculture;

            //Start SqlDependency with application initialization
            SqlDependency.Start(connString);
            //RegisterNotification();
        }
        protected void Application_End()
        {
            //Stop SQL dependency
            SqlDependency.Stop(connString);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            //initializes Cache on first request
            var x = HttpContext.Current;
        }
    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
