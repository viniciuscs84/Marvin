using System.Collections.Generic;

namespace Marvin.AppCore.Actions
{
    public class ApplicationSave : Marvin.Actions.GenericSave<Entities.Application>
    {
        public ApplicationSave(Entities.Application entity) : base(entity) { }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Code", _entity.Code);
            Entities.Application control = _dataAccess.Select<Entities.Application>(parameters);
            if (control != null && control.IsActive && control.Id != _entity.Id)
                throw new Commons.Exceptions.EssentialsException(Globalization.AppCore.Errors.AlreadyExistsApplicationCode, "AlreadyExistsApplicationCode", entity: _entity);
        }
    }
}