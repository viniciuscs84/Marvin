namespace Marvin.AppCore.Actions
{
    public class ArchiveGetFile : Marvin.Actions.ActionFacade
    {
        private bool _compressed;
        private Entities.Archive _entity;
        private Commons.Utilities.FileProvider _fileProvider;

        public Commons.Utilities.File File { get; protected set; }

        public ArchiveGetFile(Entities.Archive entity, bool compressed = false)
        {
            _compressed = compressed;
            _entity = entity;
            _fileProvider = (Commons.Utilities.FileProvider)System.Activator.CreateInstance(System.Type.GetType(_entity.ProviderClass));
        }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            if (!string.IsNullOrEmpty(_entity.ProviderArgs))
                _fileProvider.SetCredentials(_entity.ProviderArgs);
        }

        protected override void Execute()
        {
            File = _fileProvider.Get(_entity.StorageFileName, _entity.Path);
            if (File.Compressed && (!_compressed || _entity.StorageStrategy != Entities.StorageStrategy.Compressed))
            {
                File = _fileProvider.UnCompress(File, _entity.LockKey);
                File.Name = _entity.Name;
            }
        }

        protected override void PostExecute()
        {
            if (_compressed && !File.Compressed)
                File = _fileProvider.Compress(File);
        }
    }
}