/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
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

        public override string SearchArea
        {
            get { return "ttv"; }
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
                return new Type[2]
                        {
                          typeof (ContentFolder), typeof(VideoFolder)
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
          ContentReference.GlobalBlockFolder.ToString()
        };
            }
        }
    }
}