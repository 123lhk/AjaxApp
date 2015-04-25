using SimpleInjector;

namespace AjaxApp.DataAccess.Model.UserManagement
{
	public static class UserDbContextDI
	{
		public static void Confiure(Container container)
		{
			container.RegisterWebApiRequest<UserDbContext>();

		}
	}
}