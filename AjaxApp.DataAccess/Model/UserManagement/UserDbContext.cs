using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.BuilderProperties;

namespace AjaxApp.DataAccess.Model.UserManagement
{

	public class UserDbContext : IdentityDbContext<ApplicationUser>
	{
		public UserDbContext()
			: base("AppDB", throwIfV1Schema: false)
		{
		}

		public static UserDbContext Create()
		{
			return new UserDbContext();
		}
	}
}