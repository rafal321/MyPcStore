using MyPcStore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyPcStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            if (User == null) { return; } // veryfi if user is logged in
            string user_name = Context.User.Identity.Name; //  username

            // Declare array of roles
            string[] roles = null;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(y => y.Username == user_name);
                roles = db.UserRoles.Where(y => y.UserId == dto.Id).Select(y => y.Role.Name).ToArray();
            }

            IIdentity user_Identity = new GenericIdentity(user_name);
            IPrincipal new_UserObj = new GenericPrincipal(user_Identity, roles);
            Context.User = new_UserObj;     // updating Context.User
        }
    }
}
