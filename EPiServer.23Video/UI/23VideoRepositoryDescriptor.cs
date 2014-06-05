using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace EPiServer._23Video.UI
{
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class _23VideoRepositoryDescriptor : ContentRepositoryDescriptorBase
    {
        public static string RepositoryKey
        {
            get { return "23video"; }
        }

        public override string Key
        {
            get { return RepositoryKey; }
        }

        public override string Name
        {
            get { return "23Video"; }
        }

        public override IEnumerable<Type> ContainedTypes
        {
            get { return new Type[] { typeof(IContent) }; }
        }

        public override IEnumerable<Type> MainNavigationTypes
        {
            get { return new Type[] { typeof(IContent) }; }
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
                var c = ServiceLocator.Current.GetInstance<IContentLoader>();
                var roots = c.GetChildren<ContentFolder>(new ContentReference(163));
                var nextLevel = roots.FirstOrDefault();
                var roots2 = c.GetChildren<ContentFolder>(nextLevel.ContentLink);
                foreach (var data in roots2)
                {
                    yield return data.ContentLink;
                }

                //return new List<ContentReference> {  }; 
            }
        }
    }
}