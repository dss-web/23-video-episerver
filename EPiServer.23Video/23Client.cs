using EPiServer.ServiceLocation;
using EPiServer._23Video.Initialize;
using Visual;

namespace EPiServer._23Video
{
    public static class _23Client
    {
        private static _23VideoSettings _settingsRepository;

        public static _23VideoSettings _23VideoSettings {
            get
            {
                if (_settingsRepository == null)
                {
                    var repo = ServiceLocator.Current.GetInstance<_23VideoSettingsRepository>();
                    _settingsRepository = repo.LoadSettings();
                }

                return _settingsRepository;
            } }

        public static IApiProvider ApiProvider
        {
            get
            {
                //TODO: Move default values to app settings if needed.
                return new ApiProvider(_23VideoSettings.Domain,
                    _23VideoSettings.CustomerKey,
                    _23VideoSettings.CustomerSecret,
                    _23VideoSettings.AccessToken,
                    _23VideoSettings.AccessTokenSecret, true);

            }
            private set { }

        }
    }
}
