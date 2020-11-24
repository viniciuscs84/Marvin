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

        public override void SaveEntity(Entities.Application entity)
        {
            Actions.ApplicationSave facade = new Actions.ApplicationSave(entity);
            if (!facade.DoAction())
                throw facade.Exception;
        }
    }
}