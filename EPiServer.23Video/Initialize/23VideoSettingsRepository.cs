using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Helpers;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;

namespace EPiServer._23Video.Initialize
{
    [ServiceConfiguration(ServiceType=typeof(_23VideoSettingsRepository))]
    public class _23VideoSettingsRepository
    {
        private static DynamicDataStore Store
        {
            get { return typeof(_23VideoSettings).GetStore(); }
        }

        public bool SaveSettings(_23VideoSettings settings)
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

        public _23VideoSettings LoadSettings()
        {
            var currentSettings = Store.Items<_23VideoSettings>().FirstOrDefault();
            if (currentSettings == null)
            {
                currentSettings = new _23VideoSettings();
                Store.Save(currentSettings);
            }
            return currentSettings;
        }

        public bool ValidateAccessToken()
        {
            var currentSettings = Store.Items<_23VideoSettings>().FirstOrDefault();
            if (currentSettings == null)
                return false;

            // Refresh token
            //if (DateTime.Now.Subtract(currentSettings.TokenCreated).TotalSeconds >= currentSettings.TokenExpires)
            //{
            //    var webClient = new WebClient();
            //    var resultData = webClient.UploadValues("http://api.visualplatform.net/oauth/request_token",
            //        new NameValueCollection
            //        {
            //            { "client_id", currentSettings.ClientId },
            //            { "client_secret", currentSettings.ClientSecret },
            //            { "grant_type", "refresh_token" },
            //            { "refresh_token", currentSettings.RefreshToken }
            //        });
            //    var result = Json.Decode(Encoding.UTF8.GetString(resultData));

            //    currentSettings.AccessToken = result.access_token;
            //    currentSettings.TokenExpires = result.expires_in;
            //    currentSettings.TokenCreated = DateTime.Now;
            //    SaveSettings(currentSettings);

            //    return true;
            //}
            return true;
        }
    }
}