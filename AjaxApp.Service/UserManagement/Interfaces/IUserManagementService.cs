using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using AjaxApp.Api.Models;
using AjaxApp.DataAccess.Model.UserManagement;
using AjaxApp.Service.Common;
using AjaxApp.Service.UserManagement.Model;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace AjaxApp.Service.UserManagement.Interfaces
{
	public interface IUserManagementService
	{
		UserDetail GetUserById(string userId);
		ErrorCollection UpdateUser(UserDetail detail);
		ErrorCollection CreateUser(UserDetail detail, string password);

		void SignOut(string authenticationType);
		ManageInfoViewModel GetManageInfo(string userId, string localLoginProvider, string redirectUri, string publicClientId, UrlHelper urlHelper, bool generateState = false);
		IEnumerable<ExternalLoginViewModel> GetExternalLogins(string redirectUri, string publicClientId, UrlHelper urlHelper, bool generateState = false);

		ErrorCollection ChangePassword(string userId, string oldPassword, string newPassword);
		ErrorCollection SetPassword(string userId, string newPassword);
		ErrorCollection RemovePassword(string userId);

		AuthenticationTicket UnprotectToken(string token);

		ErrorCollection AddLogin(string userId, string loginProvider, string providerKey);
		ErrorCollection RemoveLogin(string userId, string loginProvider, string providerKey);

		void GetExternalLogin(string loginProvider, string providerKey, ExternalLoginData externalLogin);
		ErrorCollection RegisterExternal(UserDetail detail);

		Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context);
	}
}