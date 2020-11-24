using System;

namespace Marvin.AppCore.Business
{
    public class Archive : Layers.Business<Entities.Archive>
    {
        public void SaveModel(Entities.Archive model, byte[] content, string fileProviderClass = null, string providerArgs = null)
        {
            if (!model.IsRecorded && model.Date == DateTime.MinValue)
                model.Date = DateTime.Now;

            if (string.IsNullOrEmpty(model.ProviderClass))
            {
                if (string.IsNullOrEmpty(fileProviderClass))
                    fileProviderClass = AppSettings.GetSettingValue("DefaultFileProvider");
                model.ProviderClass = fileProviderClass;
            }
            if (string.IsNullOrEmpty(model.ProviderArgs))
            {
                if (string.IsNullOrEmpty(providerArgs))
                    providerArgs = AppSettings.GetSettingValue("DefaultFileProviderArgs");
                model.ProviderArgs = providerArgs;
            }
            Actions.ArchiveSave facade = new Actions.ArchiveSave(model, new Commons.Utilities.File(model.Name, content));
            if (!facade.DoAction())
                throw facade.Exception;
        }

        public override void DeleteModel(Entities.Archive model)
        {
            Actions.ArchiveDelete facade = new Actions.ArchiveDelete(model);
            if (!facade.DoAction())
                throw facade.Exception;
        }

        public Commons.Utilities.File GetFile(Entities.Archive model, bool compressed = false)
        {
            Actions.ArchiveGetFile facade = new Actions.ArchiveGetFile(model, compressed);
            if (!facade.DoAction())
                throw facade.Exception;
            return facade.File;
        }
    }
}
