using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicketSupport.Areas.Admin.Authorization
{
	public class AuthorizeRolesAttribute : AuthorizeAttribute
	{
		private readonly string[] _roles;

		public AuthorizeRolesAttribute(params string[] roles)
		{
			_roles = roles;
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var userRoles = (List<string>)httpContext.Session["UserRoles"];
			if (userRoles == null)
				return false;

			// Kiểm tra nếu người dùng có ít nhất một quyền yêu cầu
			return userRoles.Intersect(_roles).Any();
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			// Chuyển hướng đến Dashboard/Unauthorized
			filterContext.Result = new RedirectToRouteResult(
				new System.Web.Routing.RouteValueDictionary
				{
					{ "controller", "Dashboard" },
					{ "action", "Unauthorized" }  
				}
			);
		}
	}
}