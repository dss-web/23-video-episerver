/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using System.Configuration;
using EPiServer.ServiceLocation;
using log4net;

namespace EPiCode.TwentyThreeVideo.Provider
{
    [ServiceConfiguration(ServiceType = typeof(SettingsRepository))]
    public class SettingsRepository
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Settings LoadSettings()
        {
            Settings currentSettings = new Settings();

            currentSettings.Enabled = Enabled;
            currentSettings.Domain = Domain;
            currentSettings.ConsumerKey = ConsumerKey;
            currentSettings.ConsumerSecret = ConsumerSecret;
            currentSettings.AccessToken = AccessToken;
            currentSettings.AccessTokenSecret = AccessTokenSecret;
            currentSettings.OEmbedIsEnabled = oEmbedIsEnabled;


            return currentSettings;
        }

        private  string GetAppSetting(string identifier)
        {
            try
            {
                string result = ConfigurationManager.AppSettings.Get(identifier);
                if (String.IsNullOrEmpty(result))
                {
                    _log.Error("TwentyTreeVideo value '" + identifier + "' not set in config.");
                }

                return result;
            }
            catch(Exception ex)
            {
                _log.Error("TwentyTreeVideo value ´" + identifier + "´ not set in config", ex);
            }
            return null;
        }

        public string Enabled
        {
            get { return GetAppSetting("TwentythreeVideoEnabled"); }
            private set { }
        }

        public string Domain
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

        public bool oEmbedIsEnabled
        {
            get
            {
                var oEmbedSetting = GetAppSetting("TwentyThreeVideoEnableoEmbed");

                if (string.IsNullOrEmpty(oEmbedSetting) == false)
                {
                    return bool.Parse(oEmbedSetting);    
                }

                return false;
            }
            private set { }
        }
    }
}