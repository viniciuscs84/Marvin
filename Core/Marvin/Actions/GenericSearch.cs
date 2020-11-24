using System.Collections.Generic;

namespace Marvin.Actions
{
    public class GenericSearch<TModel> : ActionFacade
        where TModel : class, Layers.IModel
    {
        protected Layers.ModelDataAccess _dataAccess;
        protected Dictionary<string, object> _parameters;

        public Layers.ModelCollection<TModel> Result { get; protected set; }

        protected override void Execute()
        {
            Result = _dataAccess.Search<TModel>(_parameters);
        }

        public GenericSearch(Dictionary<string, object> parameters = null)
        {
            _parameters = parameters;
            _dataAccess = new Layers.ModelDataAccess();
        }
    }
}
