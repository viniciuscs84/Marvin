using System;
using System.Collections.Generic;

namespace Marvin.AppCore.Actions
{
    public class ArchiveSave : Marvin.Actions.GenericSave<Entities.Archive>
    {
        private Commons.Utilities.File _file;
        private Commons.Utilities.FileProvider _fileProvider;

        public ArchiveSave(Entities.Archive model, Commons.Utilities.File file)
            : base(model)
        {
            _file = file;
            _fileProvider = (Commons.Utilities.FileProvider)Activator.CreateInstance(Type.GetType(_model.ProviderClass));
            _fileProvider.SetCredentials(_model.ProviderArgs);
        }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            if (_model.IsRecorded && _model.StorageStrategy != Entities.StorageStrategy.Versioned)
            {
                Marvin.Actions.GenericSelect<Entities.Archive> selectFacade = new Marvin.Actions.GenericSelect<Entities.Archive>(new Dictionary<string, object>() { { "Id", _model.Id } });
                if (selectFacade.DoAction() && selectFacade.Result != null)
                {
                    Commons.Utilities.FileProvider oldFileProvider = (Commons.Utilities.FileProvider)Activator.CreateInstance(Type.GetType(selectFacade.Result.ProviderClass));
                    if (!string.IsNullOrEmpty(selectFacade.Result.ProviderArgs))
                        oldFileProvider.SetCredentials(selectFacade.Result.ProviderArgs);
                    oldFileProvider.Delete(selectFacade.Result.StorageFileName, selectFacade.Result.Path);
                }
            }

            Commons.Utilities.File fileToSave = _file;
            switch (_model.StorageStrategy)
            {
                case Entities.StorageStrategy.Compressed:
                    fileToSave = _fileProvider.Compress(_file, _model.LockKey);
                    break;
                case Entities.StorageStrategy.Locked:
                    if (string.IsNullOrEmpty(_model.LockKey))
                        _model.LockKey = _model.Name;
                    fileToSave = _fileProvider.Compress(new Commons.Utilities.File("file", _file.Content), _model.LockKey, Guid.NewGuid().ToString() + ".lock");
                    break;
                //TODO:implementar algoritimo de versionamento
                case Entities.StorageStrategy.Versioned:
                    fileToSave = _fileProvider.Compress(new Commons.Utilities.File("last", _file.Content), _model.LockKey, _model.Name + ".ver");
                    break;
            }
            _model.StorageFileName = fileToSave.Name;
            _fileProvider.Save(fileToSave, _model.Path);
            _model.StorageAddress = _fileProvider.GetFullPath(fileToSave.Name, _model.Path);
        }
    }
}