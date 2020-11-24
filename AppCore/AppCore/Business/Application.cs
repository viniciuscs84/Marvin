using System.Collections.Generic;

namespace Marvin.AppCore.Business
{
    public class Application : Layers.Business<Entities.Application>
    {
        public virtual Entities.Application GetByCode(string code)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Code", code);
            Marvin.Actions.GenericSelect<Entities.Application> facade = new Marvin.Actions.GenericSelect<Entities.Application>(parameters);
            if (!facade.DoAction())
                throw facade.Exception;
            return facade.Result;
        }

        public override void SaveModel(Entities.Application model)
        {
            Actions.ApplicationSave facade = new Actions.ApplicationSave(model);
            if (!facade.DoAction())
                throw facade.Exception;
        }
    }
}