using System;
using System.Linq;
using EPiServer.Data.Cache;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace EPiCode.TwentyThreeVideo.Provider
{
    [ServiceConfiguration(ServiceType=typeof(SettingsRepository))]
    public class SettingsRepository
    {
        private static DynamicDataStore Store
        {
            get { return typeof(Settings).GetStore(); }
        }

        public bool SaveSettings(Settings settings)
        {
            try
            {
                var currentSettings = LoadSettings();
                Store.Save(settings, currentSettings.Id);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public Settings LoadSettings()
        {
            
            var currentSettings = Store.Items<Settings>().FirstOrDefault();
            if (currentSettings == null)
            {
                currentSettings = new Settings();
                Store.Save(currentSettings);
            }
            return currentSettings;
        }
    }
}