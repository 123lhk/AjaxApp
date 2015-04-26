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

			container.Verify();

			GlobalConfiguration.Configuration.DependencyResolver =
				new SimpleInjectorWebApiDependencyResolver(container);

		}
	}

	
}