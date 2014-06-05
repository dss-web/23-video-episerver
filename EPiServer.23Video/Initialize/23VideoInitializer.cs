using System.Collections.Specialized;
using System.Linq;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer._23Video.Models;

namespace EPiServer._23Video.Initialize
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class _23VideoInitializer : IInitializableModule
    {
        public void Initialize(Framework.Initialization.InitializationEngine context)
        {
            // Register 23Video IContentData type
            var registerFolder = context.Locate.Advanced.GetInstance<SingleModelRegister<_23VideoFolder>>();
            registerFolder.RegisterType();
            
            var registerVideo = context.Locate.Advanced.GetInstance<SingleModelRegister<_23VideoVideo>>();
            registerVideo.RegisterType();
            
            var contentRepository = context.Locate.ContentRepository();

            var entryPoint = contentRepository.GetChildren<_23VideoFolder>(ContentReference.RootPage).FirstOrDefault();
            if (entryPoint == null)
            {
                entryPoint = contentRepository.GetDefault<_23VideoFolder>(ContentReference.RootPage);
                entryPoint.Name = "23Video";
                contentRepository.Save(entryPoint, DataAccess.SaveAction.Publish, Security.AccessLevel.NoAccess);
            }

            // Register custom content provider
            var providerValues = new NameValueCollection();
            providerValues.Add(ContentProviderElement.EntryPointString, entryPoint.ContentLink.ID.ToString());
            providerValues.Add(ContentProviderElement.CapabilitiesString, ContentProviderElement.FullSupportString);

            var productProvider = new _23VideoProvider(context.Locate.ContentTypeRepository(), entryPoint, ServiceLocator.Current.GetInstance< _23VideoSettingsRepository>());
            productProvider.Initialize("23Video", providerValues);

            var providerManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
            providerManager.ProviderMap.AddProvider(productProvider);

        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(Framework.Initialization.InitializationEngine context)
        {
        }
    }
}