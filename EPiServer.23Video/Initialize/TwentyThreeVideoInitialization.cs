/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EPiCode.TwentyThreeVideo.Data;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.DataAbstraction.RuntimeModel;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiCode.TwentyThreeVideo.Models;
using log4net;

namespace EPiCode.TwentyThreeVideo.Initialize
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    [ModuleDependency(typeof(DataInitialization))]
    public class TwentyThreeVideoInitialization : IInitializableModule
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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

            var domain = SettingsRepository.Service.Domain;
            WebRequest.RegisterPrefix("https://" + domain, TwentyThreeRequestCreator.TwentyThreeHttp);
            WebRequest.RegisterPrefix("http://" + domain, TwentyThreeRequestCreator.TwentyThreeHttp);

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
                entryPoint.Name = Constants.EntryPointName;
                contentRepository.Save(entryPoint, SaveAction.Publish, AccessLevel.NoAccess);
            }

            // Register custom content provider
            var providerValues = new NameValueCollection();
            providerValues.Add(ContentProviderElement.EntryPointString, entryPoint.ContentLink.ID.ToString());
            providerValues.Add(ContentProviderElement.CapabilitiesString, ContentProviderElement.FullSupportString);
            var twentyThreeVideoProvider = new TwentyThreeVideoProvider(context.Locate.ContentTypeRepository(), ServiceLocator.Current.GetInstance<IntermediateVideoDataRepository>());
            twentyThreeVideoProvider.Initialize(Constants.ProviderKey, providerValues);
            var providerManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
            providerManager.ProviderMap.AddProvider(twentyThreeVideoProvider);

            // Refresh video content
            try
            {
                Task.Run(() => Refresh(twentyThreeVideoProvider));
            }
            catch (Exception ex)
            {
                _log.Error("An error occured during video initialization:" + ex);
            }
        }

        private void Refresh(TwentyThreeVideoProvider twentyThreeVideoProvider)
        {
            var intermediateVideoDataRepository = new IntermediateVideoDataRepository();

            var items = intermediateVideoDataRepository.Load();

            if (items == null)
            {
                items = intermediateVideoDataRepository.LoadFromService();
                intermediateVideoDataRepository.Save(items);
            }

            twentyThreeVideoProvider.RefreshItems(items);
        }


        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}