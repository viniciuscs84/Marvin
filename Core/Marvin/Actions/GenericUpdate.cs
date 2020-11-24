namespace Marvin.Actions
{
    public class GenericUpdate<TModel> : ActionFacade
        where TModel : Layers.IModel
    {
        protected TModel _model;
        protected Layers.ModelDataAccess _dataAccess;

        protected override void Execute()
        {
                _dataAccess.Update(_model);
        }

        public GenericUpdate(TModel model)
        {
            _model = model;
            _dataAccess = new Layers.ModelDataAccess();
        }
    }
}
