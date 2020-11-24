using System.Collections.Generic;

namespace Marvin.Layers
{
    public class BusinessBase<TModel>
        where TModel : class, IModel
    {
        public virtual void SaveModel(TModel model)
        {
            Actions.GenericSave<TModel> facade = new Actions.GenericSave<TModel>(model);
            if (!facade.DoAction())
                throw facade.Exception;
        }

        public virtual ModelCollection<TModel> GetModelCollection(Dictionary<string, object> parameters = null)
        {
            Actions.GenericSearch<TModel> facade = new Actions.GenericSearch<TModel>(parameters);
            if (!facade.DoAction())
                throw facade.Exception;
            return facade.Result;
        }

        public virtual ModelCollection<IModel> GetNonTypedCollection(Dictionary<string, object> parameters = null)
        {
            return GetModelCollection(parameters).GetNonTypedCollection();
        }                

        public virtual void DeleteModel(TModel model)
        {
            Actions.GenericDeleteModel<TModel> facade = new Actions.GenericDeleteModel<TModel>(model);
            if (!facade.DoAction())
                throw facade.Exception;
        }

        public virtual void DeleteModelList(List<TModel> entities)
        {
            foreach (TModel model in entities)
                DeleteModel(model);
        }
    }

    public class Business<TModel, TIdmodel> : BusinessBase<TModel>
        where TModel : class, IModel
    {
        public virtual TModel GetModel(TIdmodel idmodel)
        {
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(typeof(TModel));
            List<string> keys = modelMap.GetKeys();
            if(keys != null && keys.Count > 0){
                Actions.GenericSelect<TModel> facade = new Actions.GenericSelect<TModel>(new Dictionary<string, object>() { { keys[0], idmodel } });
                if (!facade.DoAction())
                    throw facade.Exception;
                return facade.Result;
            }
            return default(TModel);
        }

        public virtual void DeleteModel(TIdmodel idmodel)
        {
            Persistence.ModelMaps.ModelMap modelMap = Persistence.ModelMaps.ModelMap.GetModelMap(typeof(TModel));
            List<string> keys = modelMap.GetKeys();
            if (keys != null && keys.Count > 0)
            {
                Actions.GenericDelete<TModel> facade = new Actions.GenericDelete<TModel>(new Dictionary<string, object>() { { keys[0], idmodel } });
                if (!facade.DoAction())
                    throw facade.Exception;
            }
        }

        public virtual void DeleteModelList(List<TIdmodel> ids)
        {
            foreach (TIdmodel id in ids)
                DeleteModel(id);
        }
    }

    public class Business<TModel> : Business<TModel, int>
        where TModel : DefaultModel
    {
    }        
}
