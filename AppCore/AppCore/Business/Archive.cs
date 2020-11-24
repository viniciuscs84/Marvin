using System;

namespace Marvin.AppCore.Business
{
    public class Archive : Layers.Business<Entities.Archive>
    {
        public void SaveEntity(Entities.Archive entity, byte[] content, string fileProviderClass = null, string providerArgs = null)
        {
            if (!entity.IsRecorded && entity.Date == DateTime.MinValue)
                entity.Date = DateTime.Now;

            if (string.IsNullOrEmpty(entity.ProviderClass))
            {
                if (string.IsNullOrEmpty(fileProviderClass))
                    fileProviderClass = AppSettings.GetSettingValue("DefaultFileProvider");
                entity.ProviderClass = fileProviderClass;
            }
            if (string.IsNullOrEmpty(entity.ProviderArgs))
            {
                if (string.IsNullOrEmpty(providerArgs))
                    providerArgs = AppSettings.GetSettingValue("DefaultFileProviderArgs");
                entity.ProviderArgs = providerArgs;
            }
            Actions.ArchiveSave facade = new Actions.ArchiveSave(entity, new Commons.Utilities.File(entity.Name, content));
            if (!facade.DoAction())
                throw facade.Exception;
        }

        public override void DeleteEntity(Entities.Archive entity)
        {
            Actions.ArchiveDelete facade = new Actions.ArchiveDelete(entity);
            if (!facade.DoAction())
                throw facade.Exception;
        }

        public Commons.Utilities.File GetFile(Entities.Archive entity, bool compressed = false)
        {
            Actions.ArchiveGetFile facade = new Actions.ArchiveGetFile(entity, compressed);
            if (!facade.DoAction())
                throw facade.Exception;
            return facade.File;
        }
    }
}
