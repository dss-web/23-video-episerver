using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiCode.TwentyThreeVideo.Models;

namespace EPiCode.TwentyThreeVideo.UI
{
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class TwentyThreeVideoRepositoryDescriptor : ContentRepositoryDescriptorBase
    {
        public static string ProvideKeyValue
        {
            get { return Constants.ProviderKey; }
        }
        public static string RepositoryKey
        {
            get { return Constants.ProviderKey; }
        }

        public override string Key
        {
            get { return RepositoryKey; }
        }

        public override string Name
        {
            get { return Constants.ProviderKey; }
        }

        public override IEnumerable<Type> ContainedTypes
        {
            get { return new Type[] { typeof(Video) }; }
        }


        public override IEnumerable<Type> MainNavigationTypes
        {
            get
            {
                return (IEnumerable<Type>)new Type[2]
        {
          typeof (ContentFolder), typeof(IContent)
        };
            }
        }


        public override int SortOrder
        {
            get { return 400; }
        }

        public bool ChangeContextOnItemSelection { get { return true; } }

        public override IEnumerable<ContentReference> Roots
        {
            get
            {
                var children =
    ServiceLocator.Current.GetInstance<IContentRepository>()
        .GetChildren<VideoFolder>(ContentReference.RootPage);

                foreach (var folder in children)
                {
                    yield return folder.ContentLink;
                }
            }
        }

        public override IEnumerable<string> PreventContextualContentFor
        {
            get
            {
                return (IEnumerable<string>)new string[3]
        {
          ContentReference.RootPage.ToString(),
          ContentReference.WasteBasket.ToString(),
          "3"
        };
            }
        }
    }
}