namespace Marvin.Actions
{
    public class GenericDeleteModel<TModel>: ActionFacade
        where TModel : Layers.IModel
    {
        protected TModel _model;
        protected Layers.ModelDataAccess _dataAccess;

        protected override void Execute()
        {
            _dataAccess.DeleteModel(_model);
        }

        public GenericDeleteModel(TModel model)
        {
            _model = model;
            _dataAccess = new Layers.ModelDataAccess();
        }
    }
}
