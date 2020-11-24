using System.Collections.Generic;

namespace Marvin.AppCore.Business
{
    public class Setting : Layers.Business<Entities.Setting>
    {
        public virtual Entities.Setting GetSettingByKey(string key, Entities.Application application)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Key", key);
            parameters.Add("Application", application.Id);
            Marvin.Actions.GenericSelect<Entities.Setting> facade = new Marvin.Actions.GenericSelect<Entities.Setting>(parameters);
            if (!facade.DoAction())
                throw facade.Exception;
            return facade.Result;
        }

        public override void SaveModel(Entities.Setting model)
        {
            Actions.SettingSave facade = new Actions.SettingSave(model);
            if (!facade.DoAction())
                throw facade.Exception;
        }
    }
}
