using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AjaxApp.DataAccess.Model.UserManagement
{
	public class ApplicationUser : IdentityUser
	{
		

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here

			return userIdentity;
		}

		public  ClaimsIdentity GenerateUserIdentity(UserManager<ApplicationUser> manager, string authenticationType)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = manager.CreateIdentity(this, authenticationType);
			// Add custom user claims here
			return userIdentity;
		}
	}
}