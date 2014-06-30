using System.Collections.Generic;
using System.Linq;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiCode.TwentyThreeVideo.UI
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentReference), UIHint = "twentythreevideo")]
    public class TwentyThreeVideoEditorDescriptor : ContentReferenceEditorDescriptor<Video> 
    {
        public override IEnumerable<ContentReference> Roots
        {
            get
            {
                var c = ServiceLocator.Current.GetInstance<IContentRepository>();
                var videoRoot = c.GetChildren<VideoFolder>(ContentReference.RootPage).FirstOrDefault();

                if (videoRoot != null)
                {
                    IEnumerable<VideoFolder> videofolders =
                        DataFactory.Instance.GetChildren<VideoFolder>(videoRoot.ContentLink);

                    foreach (var videofolder in videofolders)
                    {
                        yield return videofolder.ContentLink;
                    }
                }
            }
        }
    }

}
