using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using AjaxApp.Api.Models;
using AjaxApp.Api.Results;
using AjaxApp.Service.Common;
using AjaxApp.Service.UserManagement.Interfaces;
using AjaxApp.Service.UserManagement.Model;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;

namespace AjaxApp.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
		private readonly IUserManagementService userManagementService;

		public AccountController(IUserManagementService userManagementService)
		{
			this.userManagementService = userManagementService;

		}


        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            userManagementService.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public ManageInfoViewModel GetManageInfo(string returnUrl, bool generateState = false)
        {
	        return userManagementService.GetManageInfo(User.Identity.GetUserId(), LocalLoginProvider,
		        new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
		        Startup.PublicClientId, Url, generateState);
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

	        var result = userManagementService.ChangePassword(User.Identity.GetUserId(), model.OldPassword,
		        model.NewPassword);
            
            if (result.HasError)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public  IHttpActionResult SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

	        var result = userManagementService.SetPassword(User.Identity.GetUserId(), model.NewPassword); 

            if (result.HasError)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public IHttpActionResult AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userManagementService.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var ticket = userManagementService.UnprotectToken(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            var result = userManagementService.AddLogin(User.Identity.GetUserId(), externalData.LoginProvider, externalData.ProviderKey);

            if (result.HasError)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public IHttpActionResult RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

	        ErrorCollection result = null;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = userManagementService.RemovePassword(User.Identity.GetUserId());
            }
            else
            {
                result = userManagementService.RemoveLogin(User.Identity.GetUserId(), model.LoginProvider, model.ProviderKey);
            }

            if (result.HasError)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                userManagementService.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }
	    
	        userManagementService.GetExternalLogin(externalLogin.LoginProvider, externalLogin.ProviderKey, externalLogin);

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
	        return userManagementService.GetExternalLogins(new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
		        Startup.PublicClientId, Url, generateState);
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public IHttpActionResult Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new UserDetail() { UserName = model.Email, Email = model.Email };

            var result =  userManagementService.CreateUser(user, model.Password);

            if (result.HasError)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public  IHttpActionResult RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var detail = new UserDetail(){ UserName = model.Email, Email = model.Email };
 
            var result = userManagementService.RegisterExternal(detail);
            if (result.HasError)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

       
        #region Helpers

      
        private IHttpActionResult GetErrorResult(ErrorCollection result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (result.HasError)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

       

        

        #endregion
    }
}
