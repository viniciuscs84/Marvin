namespace Marvin.AppCore.Actions
{
    public class ArchiveDelete : Marvin.Actions.GenericDeleteModel<Entities.Archive>
    {
        private Commons.Utilities.FileProvider _fileProvider;

        public ArchiveDelete(Entities.Archive model) : base(model)
        {
            _fileProvider = (Commons.Utilities.FileProvider)System.Activator.CreateInstance(System.Type.GetType(_model.ProviderClass));
        }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            if (!string.IsNullOrEmpty(_model.ProviderArgs))
                _fileProvider.SetCredentials(_model.ProviderArgs);
        }

        protected override void PostExecute()
        {
            base.PostExecute();
            _fileProvider.Delete(_model.StorageFileName, _model.Path);
        }
    }
}