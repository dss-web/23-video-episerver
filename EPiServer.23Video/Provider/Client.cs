/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
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
