using System.Collections.Generic;

namespace Marvin.AppCore.Actions
{
    public class SettingSave : Marvin.Actions.GenericSave<Entities.Setting>
    {
        public SettingSave(Entities.Setting entity) : base(entity) { }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Key", _entity.Key);
            parameters.Add("Application", _entity.Application.Id);
            Entities.Setting control = _dataAccess.Select<Entities.Setting>(parameters);
            if (control != null && control.Id != _entity.Id)
                throw new Commons.Exceptions.EssentialsException(Globalization.AppCore.Errors.AlreadyExistsApplicationSetting, "AlreadyExistsApplicationSetting", entity: _entity);
        }
    }
}