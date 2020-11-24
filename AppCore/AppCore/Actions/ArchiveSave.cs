using System;
using System.Collections.Generic;

namespace Marvin.AppCore.Actions
{
    public class ArchiveSave : Marvin.Actions.GenericSave<Entities.Archive>
    {
        private Commons.Utilities.File _file;
        private Commons.Utilities.FileProvider _fileProvider;

        public ArchiveSave(Entities.Archive entity, Commons.Utilities.File file)
            : base(entity)
        {
            _file = file;
            _fileProvider = (Commons.Utilities.FileProvider)Activator.CreateInstance(Type.GetType(_entity.ProviderClass));
            _fileProvider.SetCredentials(_entity.ProviderArgs);
        }

        protected override void PrepareExecute()
        {
            base.PrepareExecute();
            if (_entity.IsRecorded && _entity.StorageStrategy != Entities.StorageStrategy.Versioned)
            {
                Marvin.Actions.GenericSelect<Entities.Archive> selectFacade = new Marvin.Actions.GenericSelect<Entities.Archive>(new Dictionary<string, object>() { { "Id", _entity.Id } });
                if (selectFacade.DoAction() && selectFacade.Result != null)
                {
                    Commons.Utilities.FileProvider oldFileProvider = (Commons.Utilities.FileProvider)Activator.CreateInstance(Type.GetType(selectFacade.Result.ProviderClass));
                    if (!string.IsNullOrEmpty(selectFacade.Result.ProviderArgs))
                        oldFileProvider.SetCredentials(selectFacade.Result.ProviderArgs);
                    oldFileProvider.Delete(selectFacade.Result.StorageFileName, selectFacade.Result.Path);
                }
            }

            Commons.Utilities.File fileToSave = _file;
            switch (_entity.StorageStrategy)
            {
                case Entities.StorageStrategy.Compressed:
                    fileToSave = _fileProvider.Compress(_file, _entity.LockKey);
                    break;
                case Entities.StorageStrategy.Locked:
                    if (string.IsNullOrEmpty(_entity.LockKey))
                        _entity.LockKey = _entity.Name;
                    fileToSave = _fileProvider.Compress(new Commons.Utilities.File("file", _file.Content), _entity.LockKey, Guid.NewGuid().ToString() + ".lock");
                    break;
                //TODO:implementar algoritimo de versionamento
                case Entities.StorageStrategy.Versioned:
                    fileToSave = _fileProvider.Compress(new Commons.Utilities.File("last", _file.Content), _entity.LockKey, _entity.Name + ".ver");
                    break;
            }
            _entity.StorageFileName = fileToSave.Name;
            _fileProvider.Save(fileToSave, _entity.Path);
            _entity.StorageAddress = _fileProvider.GetFullPath(fileToSave.Name, _entity.Path);
        }
    }
}