using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AjaxApp.Service.UserManagement.Implementations;
using AjaxApp.Service.UserManagement.Interfaces;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using SimpleInjector;

namespace AjaxApp.Api.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

		public static Container Container { get; set; }
	    private IUserManagementService _userManagementService = null;

	    public IUserManagementService UserManagementService
	    {
		    get
		    {
			    if (_userManagementService == null)
			    {
				    _userManagementService = Container.GetInstance<IUserManagementService>();
			    }

			    return _userManagementService;
		    }
	    }


        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;

        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
	        await UserManagementService.GrantResourceOwnerCredentials(context);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}