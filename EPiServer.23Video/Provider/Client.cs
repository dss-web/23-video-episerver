using EPiServer.ServiceLocation;
using Visual;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public static class Client
    {
        private static Settings _settingsRepository;

        public static Settings Settings
        {
            get
            {
                if (_settingsRepository == null)
                {
                    var repo = ServiceLocator.Current.GetInstance<SettingsRepository>();
                    _settingsRepository = repo.LoadSettings();
                }

                return _settingsRepository;
            }
        }

        public static IApiProvider ApiProvider
        {
            get
            {
                return new ApiProvider(Settings.Domain,
                    Settings.ConsumerKey,
                    Settings.ConsumerSecret,
                    Settings.AccessToken,
                    Settings.AccessTokenSecret, true);

            }
            private set { }

        }
    }
}
