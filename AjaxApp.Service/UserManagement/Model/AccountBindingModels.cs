using System.ComponentModel.DataAnnotations;

namespace AjaxApp.Service.UserManagement.Model
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

	public class RegisterExternalBindingModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string Provider { get; set; }

		[Required]
		public string ExternalAccessToken { get; set; }

	}

	public class ParsedExternalAccessToken
	{
		public string user_id { get; set; }
		public string app_id { get; set; }
	}
}
