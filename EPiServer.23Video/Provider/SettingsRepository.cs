/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using System.Collections.Generic;
using System.Configuration;
using EPiServer.ServiceLocation;
using log4net;

namespace EPiCode.TwentyThreeVideo.Provider
{
    [ServiceConfiguration(ServiceType = typeof(SettingsRepository))]
    public class SettingsRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Settings LoadSettings()
        {
            Settings currentSettings = new Settings
            {
                Enabled = Enabled,
                Domain = Domain,
                ConsumerKey = ConsumerKey,
                ConsumerSecret = ConsumerSecret,
                AccessToken = AccessToken,
                AccessTokenSecret = AccessTokenSecret,
                OEmbedIsEnabled = oEmbedIsEnabled,
                EditorGroupChannelMapping = EditorGroupChannelMapping
            };

            return currentSettings;
        }

        public IDictionary<string, string> EditorGroupChannelMapping
        {
            get
            { 
                var mapping = new Dictionary<string, string>();

                var setting = GetAppSetting("TwentythreeVideoEditorMapping");

                if (!string.IsNullOrWhiteSpace(setting))
                {
                   
                    foreach (var s in setting.Split('|'))
                    {
                        var t = s.Split(';');
                        if (!mapping.ContainsKey(t[0]))
                            mapping.Add(t[0], t[1]);
                    }                    
                }

                return mapping;
            }
        }

        private  string GetAppSetting(string identifier)
        {
            try
            {
                string result = ConfigurationManager.AppSettings.Get(identifier);
                if (String.IsNullOrEmpty(result))
                {
                    Log.Error("TwentyTreeVideo value '" + identifier + "' not set in config.");
                }

                return result;
            }
            catch(Exception ex)
            {
                Log.Error("TwentyTreeVideo value ´" + identifier + "´ not set in config", ex);
            }
            return null;
        }

        public string Enabled
        {
            get { return GetAppSetting("TwentythreeVideoEnabled"); }
        }

        public string Domain
        {
            get { return GetAppSetting("TwentythreeVideoDomain"); }
        }

        public  string ConsumerKey
        {
            get { return GetAppSetting("TwentythreeVideoConsumerKey"); }
        }

        public  string ConsumerSecret
        {
            get { return GetAppSetting("TwentythreeVideoConsumerSecret"); }
        }

        public  string AccessToken
        {
            get { return GetAppSetting("TwentythreeVideoAccessToken"); }
        }

        public  string AccessTokenSecret
        {
            get { return GetAppSetting("TwentythreeVideoAccessTokenSecret"); }
        }

        public int MaxDegreeOfParallelism
        {
            get
            {
                var value = GetAppSetting("TwentythreeVideoMaxDegreeOfParallelism");
                int result;
                if (int.TryParse(value, out result))
                {
                    return result;
                }
                //-1 is unlimited
                return -1;
            }
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
        }
    }
}