using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using SimpleInjector;

[assembly: OwinStartup(typeof(AjaxApp.Api.Startup))]

namespace AjaxApp.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			ConfigureDIContainer(app);
            ConfigureAuth(app);
        }
    }
}
