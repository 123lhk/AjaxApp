using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using AjaxApp.DataAccess.Model.UserManagement;
using AjaxApp.Service.Common;
using AjaxApp.Service.UserManagement.Helpers;
using AjaxApp.Service.UserManagement.Interfaces;
using AjaxApp.Service.UserManagement.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;


namespace AjaxApp.Service.UserManagement.Implementations
{
	public class UserManagementService : IUserManagementService
	{
		public static IDataProtectionProvider DataProtectionProvider { get;  set; }

		private readonly ApplicationUserManager applicationUserManager;
		//private readonly ApplicationSignInManager applicationSignInManager;
		private readonly UserMapper userMapper;
		private readonly ISecureDataFormat<AuthenticationTicket> accessTokenFormat;
		private readonly IAuthenticationManager authenticationManager;

		public UserManagementService(ApplicationUserManager applicationUserManager, 
			UserMapper userMapper, 
			ISecureDataFormat<AuthenticationTicket> accessTokenFormat, 
			IAuthenticationManager authenticationManager)
		{
			this.applicationUserManager = applicationUserManager;
			//this.applicationSignInManager = applicationSignInManager;
			this.userMapper = userMapper;
			this.accessTokenFormat = accessTokenFormat;
			this.authenticationManager = authenticationManager;
		}

		public UserDetail GetUserById(string userId)
		{
			return userMapper.MapToDetail(applicationUserManager.FindById(userId));
		}

		public UserDetail  GetUserByLoginInfo(UserLoginInfo loginInfo)
		{
			return userMapper.MapToDetail(applicationUserManager.Find(loginInfo));
		}

		public ErrorCollection UpdateUser(UserDetail detail)
		{
			var rv = new ErrorCollection();

			var user = applicationUserManager.FindById(detail.Id);

			if (user == null)
			{
				rv.Errors.Add("Invalid user id");
				return rv;
			}

			userMapper.MapToModel(detail, user);

			applicationUserManager.Update(user);

			return rv;
		}

		public ErrorCollection CreateUser(UserDetail detail, string password)
		{
			var rv = new ErrorCollection();

			var user = new ApplicationUser()
			{
				UserName = detail.Email,
				Email = detail.Email
			};

			AddFromIdentityResult(applicationUserManager.Create(user, password), rv);

			return rv;
		}

		public void SignOut(string authenticationType)
		{
			authenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
		}

		//TODO: need to decouple further
		public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string redirectUri, string publicClientId, UrlHelper urlHelper, bool generateState = false)
		{
			IEnumerable<AuthenticationDescription> descriptions = authenticationManager.GetExternalAuthenticationTypes();
			List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

			string state;

			if (generateState)
			{
				const int strengthInBits = 256;
				state = RandomOAuthStateGenerator.Generate(strengthInBits);
			}
			else
			{
				state = null;
			}

			foreach (AuthenticationDescription description in descriptions)
			{

				ExternalLoginViewModel login = new ExternalLoginViewModel
				{
					Name = description.Caption,
					
					Url = urlHelper.Route("ExternalLogin", new
					{
						provider = description.AuthenticationType,
						response_type = "token",
						client_id = publicClientId,
						redirect_uri = redirectUri,
						state = state
					}),
					State = state
				};
				logins.Add(login);
			}

			return logins;
		}

		public ErrorCollection ChangePassword(string userId, string oldPassword, string newPassword)
		{
			var rv = new ErrorCollection();

			AddFromIdentityResult(applicationUserManager.ChangePassword(userId, oldPassword, newPassword), rv);

			return rv;
		}

		public ErrorCollection SetPassword(string userId, string newPassword)
		{
			var rv = new ErrorCollection();
			AddFromIdentityResult(applicationUserManager.AddPassword(userId, newPassword), rv);

			return rv;
		}

		public ErrorCollection RemovePassword(string userId)
		{
			var rv = new ErrorCollection();

			AddFromIdentityResult(applicationUserManager.RemovePassword(userId), rv);

			return rv;
		}

		public AuthenticationTicket UnprotectToken(string token)
		{
			return accessTokenFormat.Unprotect(token);
		}

		public ErrorCollection AddLogin(string userId, string loginProvider, string providerKey)
		{
			var rv = new ErrorCollection();

			AddFromIdentityResult(applicationUserManager.AddLogin(userId, new UserLoginInfo(loginProvider, providerKey)), rv);

			return rv;
		}

		public ErrorCollection RemoveLogin(string userId, string loginProvider, string providerKey)
		{
			var rv = new ErrorCollection();

			AddFromIdentityResult(applicationUserManager.RemoveLogin(userId, new UserLoginInfo(loginProvider, providerKey)), rv);

			return rv;
		}

		public bool IsExternalLoginUserRegistered(string loginProvider, string providerKey, ExternalLoginData externalLogin)
		{
			var user = applicationUserManager.Find(new UserLoginInfo(loginProvider, providerKey));

			return user != null;
		}

		public ErrorCollection RegisterExternal(UserDetail detail, string provider)
		{
			var rv = new ErrorCollection();

			var user = applicationUserManager.Find(new UserLoginInfo(provider, detail.UserName));

			if (user != null)
			{
				rv.Errors.Add("this user has been registered already.");
				return rv;
			}

			user = new ApplicationUser() { UserName = detail.Email, Email = detail.Email };

			AddFromIdentityResult(applicationUserManager.Create(user), rv);

			if (rv.HasError)
				return rv;

			var info = new ExternalLoginInfo()
			{
				DefaultUserName = user.UserName,
				Login = new UserLoginInfo(provider, user.UserName)
			};

			AddFromIdentityResult(applicationUserManager.AddLogin(user.Id, info.Login), rv);

			return rv;
		}

		public async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			var user = await applicationUserManager.FindAsync(context.UserName, context.Password);

			if (user == null)
			{
				context.SetError("invalid_grant", "The user name or password is incorrect.");
				return;
			}

			ClaimsIdentity oAuthIdentity =  user.GenerateUserIdentity(applicationUserManager,
			   OAuthDefaults.AuthenticationType);
			ClaimsIdentity cookiesIdentity =  user.GenerateUserIdentity(applicationUserManager,
				CookieAuthenticationDefaults.AuthenticationType);

			AuthenticationProperties properties = CreateProperties(user.UserName);
			AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
			context.Validated(ticket);
			context.Request.Context.Authentication.SignIn(cookiesIdentity);
		}

		private static AuthenticationProperties CreateProperties(string userName)
		{
			IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
			return new AuthenticationProperties(data);
		}

		//TODO: need to decouple further
		public ManageInfoViewModel GetManageInfo(string userId, string localLoginProvider, string redirectUri, string publicClientId, UrlHelper urlHelper, bool generateState = false)
		{
			IdentityUser user = applicationUserManager.FindById(userId);

			if (user == null)
			{
				return null;
			}

			List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

			foreach (IdentityUserLogin linkedAccount in user.Logins)
			{
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = linkedAccount.LoginProvider,
					ProviderKey = linkedAccount.ProviderKey
				});
			}

			if (user.PasswordHash != null)
			{
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = localLoginProvider,
					ProviderKey = user.UserName,
				});
			}

			return new ManageInfoViewModel
			{
				LocalLoginProvider = localLoginProvider,
				Email = user.UserName,
				Logins = logins,
				ExternalLoginProviders = GetExternalLogins(redirectUri, publicClientId, urlHelper, generateState)
			};
		}

		private static class RandomOAuthStateGenerator
		{
			private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

			public static string Generate(int strengthInBits)
			{
				const int bitsPerByte = 8;

				if (strengthInBits % bitsPerByte != 0)
				{
					throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
				}

				int strengthInBytes = strengthInBits / bitsPerByte;

				byte[] data = new byte[strengthInBytes];
				_random.GetBytes(data);
				return HttpServerUtility.UrlTokenEncode(data);
			}
		}



		private void AddFromIdentityResult(IdentityResult result, ErrorCollection error)
		{
			if (result.Succeeded)
			{
				return;
			}

			if (result.Errors != null && result.Errors.Any())
			{
				error.Errors.AddRange(result.Errors);
			}
			else
			{
				throw new ApplicationException("Unexpected Error!!!");
			}
		}
	}
}