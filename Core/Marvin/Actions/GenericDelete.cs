using System.Collections.Generic;

namespace Marvin.Actions
{
    public class GenericDelete<TModel>: ActionFacade
        where TModel : class, Layers.IModel
    {
        protected Layers.ModelDataAccess _dataAccess;
        protected Dictionary<string, object> _parameters;

        protected override void Execute()
        {
            _dataAccess.Delete<TModel>(_parameters);
        }

        public GenericDelete(Dictionary<string, object> parameters = null)
        {
            _parameters = parameters;
            _dataAccess = new Layers.ModelDataAccess();
        }
    }
}
