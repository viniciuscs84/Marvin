namespace Marvin.AppCore.Actions
{
    public class ArchiveGetFile : Marvin.Actions.ActionFacade
    {
        private bool _compressed;
        private Entities.Archive _model;
        private Commons.Utilities.FileProvider _fileProvider;

        public Commons.Utilities.File File { get; protected set; }

        public ArchiveGetFile(Entities.Archive model, bool compressed = false)
        {
            _compressed = compressed;
            _model = model;
            _fileProvider = (Commons.Utilities.FileProvider)System.Activator.CreateInstance(System.Type.GetType(_model.ProviderClass));
        }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            if (!string.IsNullOrEmpty(_model.ProviderArgs))
                _fileProvider.SetCredentials(_model.ProviderArgs);
        }

        protected override void Execute()
        {
            File = _fileProvider.Get(_model.StorageFileName, _model.Path);
            if (File.Compressed && (!_compressed || _model.StorageStrategy != Entities.StorageStrategy.Compressed))
            {
                File = _fileProvider.UnCompress(File, _model.LockKey);
                File.Name = _model.Name;
            }
        }

        protected override void PostExecute()
        {
            if (_compressed && !File.Compressed)
                File = _fileProvider.Compress(File);
        }
    }
}