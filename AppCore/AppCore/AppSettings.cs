using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.AppCore
{
    public class AppSettings
    {
        protected static Dictionary<string, string> DefaultSettings = new Dictionary<string, string>(){
            {"AppTitle","Marvin Essentials"},
            {"AppTrendMark", "Marvin Essentials!"},
            {"AppCompanyWebSite", "www.marvin.com.br"},
            {"AppCompany", "Marvin IT House"},
            {"AppAuthPasswordMinSize", "6"},
            {"AppAuthRequired", "true"},
            {"DefaultFileProvider", "Marvin.Commons.Utilities.LocalFileProvider,Marvin.Commons" },
            {"DefaultFileProviderArgs", "root:/AppUpload" }
        };

        public static string GetSettingValue(string key, Entities.Application application = null)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            if (application == null)
                application = RunningApplication;

            if (key != "AppKey" && key != "AppCode")
            {
                Business.Setting business = new Business.Setting();
                Entities.Setting setting = business.GetSettingByKey(key, application);
                if (setting != null)
                    return setting.Value;
            }

            System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
            try
            {
                string value = settingsReader.GetValue(key, typeof(string)).ToString();
                return value;
            }
            catch
            {
                if (DefaultSettings.ContainsKey(key))
                    return DefaultSettings[key];
                return null;
            }
        }

        public static string AppName
        {
            get
            {
                return RunningApplication != null ? RunningApplication.Name : null;
            }
        }

        public static string AppTitle
        {
            get
            {
                return (RunningApplication != null && !string.IsNullOrEmpty(RunningApplication.Title)) ? RunningApplication.Title : GetSettingValue("AppTitle");
            }
        }

        public static string AppEmail
        {
            get { return GetSettingValue("AppEmail"); }
        }

        public static string AppSupportEmail
        {
            get { return GetSettingValue("AppSupportEmail"); }
        }

        public static string AppTrendMark
        {
            get { return GetSettingValue("AppTrendMark"); }
        }

        public static string AppCompanyWebSite
        {
            get { return GetSettingValue("AppCompanyWebSite"); }
        }

        public static string AppCompany
        {
            get { return GetSettingValue("AppCompany"); }
        }

        public static bool AppAuthUseEmail
        {
            get
            {
                return Convert.ToBoolean(GetSettingValue("AppAuthUseEmail"));
            }
        }

        public static bool AppAuthUseComplexPassword
        {
            get
            {
                return Convert.ToBoolean(GetSettingValue("AppAuthUseComplexPassword"));
            }
        }

        public static int AppAuthPasswordBufferSize
        {
            get
            {
                return Convert.ToInt32(GetSettingValue("AppAuthPasswordBufferSize"));
            }
        }

        public static bool AppAuthRequired
        {
            get
            {
                return Convert.ToBoolean(GetSettingValue("AppAuthRequired"));
            }
        }

        public static int AppAuthPasswordMinSize
        {
            get
            {
                return Convert.ToInt32(GetSettingValue("AppAuthPasswordMinSize"));
            }
        }

        public static bool AppAuthAllowProviderRegister
        {
            get
            {
                if (GetSettingValue("AppAuthAllowProviderRegister") != null)
                    return Convert.ToBoolean(GetSettingValue("AppAuthAllowProviderRegister"));
                return false;
            }
        }

        protected static Entities.Application _runningApplication;
        public static Entities.Application RunningApplication
        {
            get
            {
                if (_runningApplication == null)
                    LoadRunningApp();
                return _runningApplication;
            }
        }

        protected static void LoadRunningApp()
        {
            Commons.Utilities.Logger.Info("Loading App Settings...");
            System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
            string appCode = null;
            string appKey = null;
            try
            {
                appCode = settingsReader.GetValue("AppCode", typeof(string)).ToString();
                appKey = settingsReader.GetValue("AppKey", typeof(string)).ToString();
            }
            catch { }
            Commons.Utilities.Logger.Info("AppCode: " + appCode);
            Commons.Utilities.Logger.Info("AppKey: " + appKey);

            if (string.IsNullOrEmpty(appCode))
                throw new Commons.Exceptions.EssentialsException(Globalization.Errors.NotDefinedAppCode, "NotDefinedAppCode");
            if (string.IsNullOrEmpty(appKey))
                throw new Commons.Exceptions.EssentialsException(Globalization.Errors.NotDefinedAppKey, "NotDefinedAppKey");
            _runningApplication = new Business.Application().GetByCode(appCode);
            if (_runningApplication == null)
                throw new Commons.Exceptions.EssentialsException(Globalization.Errors.AppNotFound, "AppNotFound");
            if (_runningApplication.Key != appKey)
            {
                _runningApplication = null;
                throw new Commons.Exceptions.EssentialsException(Globalization.Errors.AppNoMatchKey, "AppNoMatchKey");
            }

            Commons.Utilities.Logger.Info("AppName: " + _runningApplication.Name);
            LogAllSettings(_runningApplication);

        }

        public static void InitApp()
        {
            LoadRunningApp();
        }

        private static void LogAllSettings(Entities.Application application)
        {
            List<string> settingsKeys = new List<string>();
            settingsKeys.AddRange(DefaultSettings.Keys);
            settingsKeys.AddRange(System.Configuration.ConfigurationManager.AppSettings.AllKeys.ToList().Where(k => !settingsKeys.Contains(k)));

            Business.Setting business = new Business.Setting();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Application", application.Id);
            Layers.ModelCollection<Entities.Setting> settings = business.GetModelCollection(parameters);

            System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();

            settingsKeys.Remove("AppCode");
            settingsKeys.Remove("AppKey");
            settingsKeys.Remove("AppName");

            foreach (string key in settingsKeys.Where(k => settings.Count(s => s.Key == k) == 0))
            {
                try
                {
                    Commons.Utilities.Logger.Info(key + ": " + settingsReader.GetValue(key, typeof(string)).ToString() + " [App config file setting]");
                }
                catch
                {
                    Commons.Utilities.Logger.Info(key + ": " + DefaultSettings[key] + " [App default settings]");
                }
            }

            foreach (Entities.Setting setting in settings)
            {
                if (setting.Key != "AppCode" && setting.Key != "AppKey" && setting.Key != "AppName")
                    Commons.Utilities.Logger.Info(setting.Key + ": " + setting.Value + " [App settings repository]");
            }
        }
    }
}
