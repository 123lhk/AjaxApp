using System;
using AjaxApp.DataAccess.Model.UserManagement;
using AjaxApp.Service.UserManagement.Implementations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace AjaxApp.Service.UserManagement.Helpers
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
			// Configure validation logic for usernames
			this.UserValidator = new UserValidator<ApplicationUser>(this)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Configure validation logic for passwords
			this.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = false,
				RequireDigit = false,
				RequireLowercase = false,
				RequireUppercase = false,
			};

			// Configure user lockout defaults
			//this.UserLockoutEnabledByDefault = true;
			//this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			//this.MaxFailedAccessAttemptsBeforeLockout = 5;

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug it in here.
			//this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
			//{
			//	MessageFormat = "Your security code is {0}"
			//});
			//this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
			//{
			//	Subject = "Security Code",
			//	BodyFormat = "Your security code is {0}"
			//});
			//this.EmailService = new EmailService();
			//this.SmsService = new SmsService();
			var dataProtectionProvider = UserManagementService.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				this.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
		}

	
	}
}