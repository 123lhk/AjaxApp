namespace AjaxApp.Service.Common
{
	public interface IViewMapper<TDetail, TModel>
	{
		TDetail MapToDetail(TModel model);

		void MapToModel(TDetail detail, TModel model);
	}
}