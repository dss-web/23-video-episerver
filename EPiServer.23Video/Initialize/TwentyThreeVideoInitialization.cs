using System.Collections.Specialized;
using System.Linq;
using System.Web.WebPages;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiCode.TwentyThreeVideo.Models;

namespace EPiCode.TwentyThreeVideo.Initialize
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class TwentyThreeVideoInitialization : EPiServer.Framework.IInitializableModule
    {

        protected Injected<SettingsRepository> SettingsRepository { get; set; }

        public void Initialize(InitializationEngine context)
        {

            if (Client.Settings.Enabled != null)
            {
                bool enabled = false;

                if (bool.TryParse(Client.Settings.Enabled, out enabled))
                {
                    // Turn off the provider
                    if (enabled == false)
                    {
                        return;
                    }
                }
            }

            // Register 23Video IContentData type
            var registerFolder = context.Locate.Advanced.GetInstance<SingleModelRegister<VideoFolder>>();
            registerFolder.RegisterType();
            var registerVideo = context.Locate.Advanced.GetInstance<SingleModelRegister<Video>>();
            registerVideo.RegisterType();
            var contentRepository = context.Locate.ContentRepository();
            var entryPoint = contentRepository.GetChildren<VideoFolder>(ContentReference.RootPage).FirstOrDefault();
            if (entryPoint == null)
            {
                AddVideoFolderToAvailablePageTypes();

                entryPoint = contentRepository.GetDefault<VideoFolder>(ContentReference.RootPage);
                entryPoint.Name = "23Video";
                contentRepository.Save(entryPoint, SaveAction.Publish, AccessLevel.NoAccess);
            }

            // Register custom content provider
            var providerValues = new NameValueCollection();
            providerValues.Add(ContentProviderElement.EntryPointString, entryPoint.ContentLink.ID.ToString());
            providerValues.Add(ContentProviderElement.CapabilitiesString, ContentProviderElement.FullSupportString);

            var twentyThreeVideoProvider = new TwentyThreeVideoProvider(context.Locate.ContentTypeRepository(), ServiceLocator.Current.GetInstance<ThumbnailManager>(), entryPoint, ServiceLocator.Current.GetInstance<SettingsRepository>());

            twentyThreeVideoProvider.Initialize("23Video", providerValues);
            var providerManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
            providerManager.ProviderMap.AddProvider(twentyThreeVideoProvider);

        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        private void AddVideoFolderToAvailablePageTypes()
        {
            var locator = ServiceLocator.Current;
            var contentTypeRepository = locator.GetInstance<IContentTypeRepository>();
            var sysRoot = contentTypeRepository.Load("SysRoot") as PageType;
            var videoFolder = contentTypeRepository.Load(typeof(VideoFolder));
            var setting = new AvailableSetting { Availability = Availability.Specific };

            if (setting.AllowedContentTypeNames.IndexOf(videoFolder.Name) == -1)
            {
                setting.AllowedContentTypeNames.Add(videoFolder.Name);
                var availabilityRepository = locator.GetInstance<IAvailableSettingsRepository>();
                availabilityRepository.RegisterSetting(sysRoot, setting);
            }
        }
    }
}