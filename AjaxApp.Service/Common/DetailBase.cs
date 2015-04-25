namespace AjaxApp.Service.Common
{
	public class DetailBase
	{
		public DetailBase()
		{
			ErrorCollection = new ErrorCollection();
		}

		public ErrorCollection ErrorCollection { get; set; }
	}
}