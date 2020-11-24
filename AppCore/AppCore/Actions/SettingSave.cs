using System.Collections.Generic;

namespace Marvin.AppCore.Actions
{
    public class SettingSave : Marvin.Actions.GenericSave<Entities.Setting>
    {
        public SettingSave(Entities.Setting model) : base(model) { }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Key", _model.Key);
            parameters.Add("Application", _model.Application.Id);
            Entities.Setting control = _dataAccess.Select<Entities.Setting>(parameters);
            if (control != null && control.Id != _model.Id)
                throw new Commons.Exceptions.EssentialsException(Globalization.AppCore.Errors.AlreadyExistsApplicationSetting, "AlreadyExistsApplicationSetting", model: _model);
        }
    }
}