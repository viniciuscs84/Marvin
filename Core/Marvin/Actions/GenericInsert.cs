namespace Marvin.Actions
{
    public class GenericInsert<TModel> : ActionFacade
        where TModel : Layers.IModel
    {
        protected TModel _model;
        protected Layers.ModelDataAccess _dataAccess;

        protected override void Execute()
        {
                _dataAccess.Insert(_model);
        }

        public GenericInsert(TModel model)
        {
            _model = model;
            _dataAccess = new Layers.ModelDataAccess();
        }
    }
}
