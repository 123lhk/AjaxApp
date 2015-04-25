using System;
using AjaxApp.Service.Common;

namespace AjaxApp.Service.UserManagement.Model
{
	public class UserDetail : DetailBase
	{

		public string Id { get; set; }

		public string UserName { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public string Email { get; set; }
		
	
	}

}