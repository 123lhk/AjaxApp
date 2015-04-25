using System.Collections.Generic;
using System.Web;
using AjaxApp.DataAccess.Model.UserManagement;
using AjaxApp.Service.UserManagement.Helpers;
using AjaxApp.Service.UserManagement.Implementations;
using AjaxApp.Service.UserManagement.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.DataProtection;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace AjaxApp.Service.UserManagement.DIConfig
{
	public static class UserManagementDI
	{
		public static void Confiure(Container container)
		{
			UserDbContextDI.Confiure(container);

			container.RegisterWebApiRequest<IUserStore<ApplicationUser>>(() =>
				new UserStore<ApplicationUser>(
					container.GetInstance<UserDbContext>()));

			container.RegisterWebApiRequest<ApplicationUserManager>();

			//container.RegisterWebApiRequest<ApplicationSignInManager>();

			container.RegisterWebApiRequest<IAuthenticationManager>(
				() =>
					container.IsVerifying()
						? new OwinContext(new Dictionary<string, object>()).Authentication
						: HttpContext.Current.GetOwinContext().Authentication);

			container.RegisterWebApiRequest<IUserManagementService, UserManagementService>();

			container.RegisterSingle<UserMapper>();

			container.Register<IDataSerializer<AuthenticationTicket>, TicketSerializer>();
			container.Register<IDataProtector>(() => new DpapiDataProtectionProvider().Create("ASP.NET Identity"));
			container.Register<ITextEncoder, Base64UrlTextEncoder>();

			container.Register<ISecureDataFormat<AuthenticationTicket>, SecureDataFormat<AuthenticationTicket>>();
		}
	}
}