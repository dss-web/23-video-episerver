using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiCode.TwentyThreeVideo.Models;
using Settings = EPiCode.TwentyThreeVideo.Provider.Settings;

namespace EPiCode.TwentyThreeVideo.Initialize
{
      [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class TwentyThreeVideoInitialization : EPiServer.Framework.IInitializableModule
    {

        protected Injected<SettingsRepository> SettingsRepository { get; set; } 

        public void Initialize(InitializationEngine context)
        {

            //if(Client.ApiProvider == null)
            //   return;
            // Register 23Video IContentData type
            var registerFolder = context.Locate.Advanced.GetInstance<SingleModelRegister<VideoFolder>>();
            registerFolder.RegisterType();
            var registerVideo = context.Locate.Advanced.GetInstance<SingleModelRegister<Video>>();
            registerVideo.RegisterType();
            var contentRepository = context.Locate.ContentRepository();
            var entryPoint = contentRepository.GetChildren<VideoFolder>(ContentReference.RootPage).FirstOrDefault();
            if (entryPoint == null)
            {
                entryPoint = contentRepository.GetDefault<VideoFolder>(ContentReference.RootPage);
                entryPoint.Name = "23Video";
                contentRepository.Save(entryPoint, SaveAction.Publish, AccessLevel.NoAccess);
            }

            // Register custom content provider
            var providerValues = new NameValueCollection();
            providerValues.Add(ContentProviderElement.EntryPointString, entryPoint.ContentLink.ID.ToString());
            providerValues.Add(ContentProviderElement.CapabilitiesString, ContentProviderElement.FullSupportString);

            var productProvider = new TwentyThreeVideoProvider(context.Locate.ContentTypeRepository(), ServiceLocator.Current.GetInstance< ThumbnailManager>(), entryPoint, ServiceLocator.Current.GetInstance<SettingsRepository>());


            productProvider.Initialize("23Video", providerValues);

            var providerManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
            providerManager.ProviderMap.AddProvider(productProvider);

        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}