using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace TicketSupport.Filters
{
    public class MyAuthenFilter : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //if (filterContext.HttpContext.User.Identity.IsAuthenticated == false)
            //{
            //    filterContext.Result = new HttpUnauthorizedResult();
            //}
            var userId = filterContext.HttpContext.Session["UserId"];
            if (userId == null)
            {
                filterContext.Result = new RedirectResult("/Admin/Login/Index");
            }

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            
        }
    }
}