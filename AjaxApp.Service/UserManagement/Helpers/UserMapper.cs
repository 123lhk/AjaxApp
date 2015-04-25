using AjaxApp.DataAccess.Model.UserManagement;
using AjaxApp.Service.Common;
using AjaxApp.Service.UserManagement.Model;
using Microsoft.Owin.BuilderProperties;

namespace AjaxApp.Service.UserManagement.Helpers
{
	public class UserMapper : IViewMapper<UserDetail, ApplicationUser>
	{
		public UserDetail MapToDetail(ApplicationUser model)
		{
			var userDetail = new UserDetail()
			{
				Id = model.Id,
				UserName = model.UserName,
				Email = model.Email	
			};

			return userDetail;

		}

		public void MapToModel(UserDetail detail, ApplicationUser model)
		{
		

		}

	
	}
}