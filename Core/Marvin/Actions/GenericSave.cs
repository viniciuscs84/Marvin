namespace Marvin.Actions
{
    public class GenericSave<TModel> : ActionFacade
        where TModel : Layers.IModel
    {
        protected TModel _model;
        protected Layers.ModelDataAccess _dataAccess;

        protected override void Execute()
        {
            if (_model.IsRecorded)
                _dataAccess.Update(_model);
            else
                _dataAccess.Insert(_model);
        }

        public GenericSave(TModel model)
        {
            _model = model;
            _dataAccess = new Layers.ModelDataAccess();
        }
    }
}
