using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using AjaxApp.Api.Results;
using AjaxApp.Service.Common;
using AjaxApp.Service.UserManagement.Interfaces;
using AjaxApp.Service.UserManagement.Model;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;

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


       

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            userManagementService.SignOut(CookieAuthenticationDefaults.AuthenticationType);
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

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public IHttpActionResult GetExternalLogin(string provider, string error = null)
        {
			string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

			var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

			if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
			{
				return BadRequest(redirectUriValidationResult);
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
	    
	        bool hasRegistered = userManagementService.IsExternalLoginUserRegistered(externalLogin.LoginProvider, externalLogin.UserName, externalLogin);
			
			redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}",
													   redirectUri,
													   externalLogin.ExternalAccessToken,
													   externalLogin.LoginProvider,
													   hasRegistered.ToString(),
													   externalLogin.UserName);

			return Redirect(redirectUri);
 
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
		[AllowAnonymous]
        public  async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
			if (verifiedAccessToken == null)
			{
				return BadRequest("Invalid Provider or External Access Token");
			}
 

            var detail = new UserDetail(){ UserName = model.Email, Email = model.Email };
 
            var result = userManagementService.RegisterExternal(detail, model.Provider);
            if (result.HasError)
            {
                return GetErrorResult(result); 
            }

	        var accessTokenReponse = GenerateLocalAccessTokenResponse(model.Email);
            return Ok(accessTokenReponse);
        }

		[AllowAnonymous]
		[HttpGet]
		[Route("ObtainLocalAccessToken")]
		public async Task<IHttpActionResult>  ObtainLocalAccessToken(string provider, string externalAccessToken)
		{

			if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
			{
				return BadRequest("Provider or external access token is not sent");
			}

			var verifiedAccessToken =  await VerifyExternalAccessToken(provider, externalAccessToken);
			if (verifiedAccessToken == null)
			{
				return BadRequest("Invalid Provider or External Access Token");
			}

			var user =  userManagementService.GetUserByLoginInfo(new UserLoginInfo(provider, verifiedAccessToken.user_id));

			bool hasRegistered = user != null;

			if (!hasRegistered)
			{
				return BadRequest("External user is not registered");
			}

			//generate access token response
			var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);

			return Ok(accessTokenResponse);

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
                        ModelState.AddModelError("ServiceError", error);
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

		private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
		{

			Uri redirectUri;

			var redirectUriString = GetQueryString(Request, "redirect_uri");

			if (string.IsNullOrWhiteSpace(redirectUriString))
			{
				return "redirect_uri is required";
			}

			bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

			if (!validUri)
			{
				return "redirect_uri is invalid";
			}

			var responseType = GetQueryString(Request, "responseType");

			if (string.IsNullOrWhiteSpace(responseType))
			{
				return "responseType is required";
			}

			if (responseType != "token")
			{
				return "Only token authentication type is supported";
			}

			redirectUriOutput = redirectUri.AbsoluteUri;

			return string.Empty;

		}

		private string GetQueryString(HttpRequestMessage request, string key)
		{
			var queryStrings = request.GetQueryNameValuePairs();

			if (queryStrings == null) return null;

			var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

			if (string.IsNullOrEmpty(match.Value)) return null;

			return match.Value;
		}

		private JObject GenerateLocalAccessTokenResponse(string userName)
		{
			var tokenExpiration = Startup.OAuthOptions.AccessTokenExpireTimeSpan;

			ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

			identity.AddClaim(new Claim(ClaimTypes.Name, userName));
			identity.AddClaim(new Claim("role", "user"));

			var props = new AuthenticationProperties()
			{
				IssuedUtc = DateTime.UtcNow,
				ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
			};

			var ticket = new AuthenticationTicket(identity, props);

			var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

			JObject tokenResponse = new JObject(
										new JProperty("userName", userName),
										new JProperty("access_token", accessToken),
										new JProperty("token_type", "bearer"),
										new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
										new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
										new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
			);

			return tokenResponse;
		}


		private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
		{
			ParsedExternalAccessToken parsedToken = null;

			var verifyTokenEndPoint = "";

			if (provider == "Google")
			{
				verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
			}
			else
			{
				return null;
			}

			var client = new HttpClient();
			var uri = new Uri(verifyTokenEndPoint);
			var response = await client.GetAsync(uri);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();

				dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

				parsedToken = new ParsedExternalAccessToken();

				if (provider == "Google")
				{
					parsedToken.user_id = jObj["email"];
					parsedToken.app_id = jObj["audience"];

					if (!string.Equals(Startup.GoogleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
					{
						return null;
					}

				}
				else
				{
					throw new NotImplementedException();
				}


			}

			return parsedToken;
		}
	}
}
