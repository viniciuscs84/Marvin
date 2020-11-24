namespace Marvin.AppCore.Actions
{
    public class ArchiveDelete : Marvin.Actions.GenericDeleteEntity<Entities.Archive>
    {
        private Commons.Utilities.FileProvider _fileProvider;

        public ArchiveDelete(Entities.Archive entity) : base(entity)
        {
            _fileProvider = (Commons.Utilities.FileProvider)System.Activator.CreateInstance(System.Type.GetType(_entity.ProviderClass));
        }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            if (!string.IsNullOrEmpty(_entity.ProviderArgs))
                _fileProvider.SetCredentials(_entity.ProviderArgs);
        }

        protected override void PostExecute()
        {
            base.PostExecute();
            _fileProvider.Delete(_entity.StorageFileName, _entity.Path);
        }
    }
}