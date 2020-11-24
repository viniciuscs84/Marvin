﻿using System.Collections.Generic;

namespace Marvin.Actions
{
    public class GenericSelect<TModel>: ActionFacade
        where TModel : class, Layers.IModel
    {
        protected Layers.ModelDataAccess _dataAccess;
        protected Dictionary<string, object> _parameters;

        public TModel Result { get; protected set; }

        protected override void Execute()
        {
            Result = _dataAccess.Select<TModel>(_parameters);
        }

        public GenericSelect(Dictionary<string, object> parameters = null)
        {
            _parameters = parameters;
            _dataAccess = new Layers.ModelDataAccess();
        }
    }
}
