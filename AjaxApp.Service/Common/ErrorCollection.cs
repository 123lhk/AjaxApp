using System.Collections.Generic;
using System.Linq;

namespace AjaxApp.Service.Common
{
	public class ErrorCollection
	{
		public ErrorCollection()
		{
			 Errors = new List<string>();
		}

		public List<string> Errors { get; set; }

		public bool HasError
		{
			get { return Errors.Any(); }
		}
	}
}