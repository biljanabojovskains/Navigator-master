using System;
using System.Security.Principal;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Navigator.Models.Concrete;
using Newtonsoft.Json;
using Westwind.Globalization;

namespace Navigator
{
    public class Global : HttpApplication
    {
        public static ApplicationAssemblyDetails ApplicationAssemblyDetails { get; set; }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //West Wind Globalization
            DbResourceConfiguration.Current.DbResourceDataManagerType = typeof(DbResourceSqLiteDataManager);     
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            ApplicationAssemblyDetails = ApplicationAssemblyDetails.Current;
            if (HttpContext.Current.User == null) return;
            if (!HttpContext.Current.User.Identity.IsAuthenticated) return;
            var id = HttpContext.Current.User.Identity as FormsIdentity;
            if (id == null) return;
            var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
            var roles = new[] { user.UserRole.RoleName };
            HttpContext.Current.User = new GenericPrincipal(id, roles);
        }
    }
}