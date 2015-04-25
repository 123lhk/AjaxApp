using System.Web.Http;
using AjaxApp.Api.Providers;
using AjaxApp.Service.UserManagement.DIConfig;
using AjaxApp.Service.UserManagement.Implementations;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.WebApi;

namespace AjaxApp.Api
{
	public partial class Startup
	{

		public void ConfigureDIContainer(IAppBuilder app)
		{
			//hacky way
			UserManagementService.DataProtectionProvider = app.GetDataProtectionProvider();

			var container = new Container();
			ApplicationOAuthProvider.Container = container;
			//Register service layer services 
			UserManagementDI.Confiure(container);

			container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

			//Important!!Create a owin middleware to make non-api request work
			app.UseOwinContextInjector(container);

			container.Verify();

			GlobalConfiguration.Configuration.DependencyResolver =
				new SimpleInjectorWebApiDependencyResolver(container);

		}
	}

	internal static class AppBUilderExtension
	{
		public static void UseOwinContextInjector(this IAppBuilder app, Container container)
		{
			// Create an OWIN middleware to create an execution context scope
			app.Use(async (context, next) =>
			{
				using (var scope = container.BeginExecutionContextScope())
				{
					await next.Invoke();
				}
			});
		}
	}
}