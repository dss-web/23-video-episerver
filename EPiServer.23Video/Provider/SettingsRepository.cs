using System;
using System.Configuration;
using Castle.Windsor.Installer;
using EPiServer.ServiceLocation;

namespace EPiCode.TwentyThreeVideo.Provider
{
    [ServiceConfiguration(ServiceType = typeof(SettingsRepository))]
    public class SettingsRepository
    {

        public Settings LoadSettings()
        {
            Settings currentSettings = new Settings();

            currentSettings.Enabled = Enabled;
            currentSettings.Domain = Domain;
            currentSettings.ConsumerKey = ConsumerKey;
            currentSettings.ConsumerSecret = ConsumerSecret;
            currentSettings.AccessToken = AccessToken;
            currentSettings.AccessTokenSecret = AccessTokenSecret;

            return currentSettings;
        }

        private  string GetAppSetting(string identifier)
        {
            try
            {
                string result = ConfigurationManager.AppSettings.Get(identifier);
                if (String.IsNullOrEmpty(result)) throw new Exception("TwentyTreeVideo value '" + identifier + "' not set in config.");
                return result;
            }
            catch
            {
                throw new Exception("TwentyTreeVideo value ´" + identifier + "´ not set in config");
            }
        }

        public string Enabled
        {
            get { return GetAppSetting("TwentythreeVideoEnabled"); }
            private set { }
        }

        public  string Domain
        {
            get { return GetAppSetting("TwentythreeVideoDomain"); }
            private set { }
        }

        public  string ConsumerKey
        {
            get { return GetAppSetting("TwentythreeVideoConsumerKey"); }
            private set { }
        }

        public  string ConsumerSecret
        {
            get { return GetAppSetting("TwentythreeVideoConsumerSecret"); }
            private set { }
        }

        public  string AccessToken
        {
            get { return GetAppSetting("TwentythreeVideoAccessToken"); }
            private set { }
        }

        public  string AccessTokenSecret
        {
            get { return GetAppSetting("TwentythreeVideoAccessTokenSecret"); }
            private set { }
        }
    }
}