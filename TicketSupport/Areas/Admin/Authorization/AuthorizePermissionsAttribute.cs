using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TicketSupport.Areas.Admin.Authorization
{
	public class AuthorizePermissionsAttribute : AuthorizeAttribute
	{
		private readonly string[] _permissions;

		public AuthorizePermissionsAttribute(params string[] permissions)
		{
			_permissions = permissions;
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var userpermissions = (List<string>)httpContext.Session["UserPermissions"];
			if (userpermissions == null)
				return false;

			// Kiểm tra nếu người dùng có ít nhất một quyền yêu cầu
			return userpermissions.Intersect(_permissions).Any();
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