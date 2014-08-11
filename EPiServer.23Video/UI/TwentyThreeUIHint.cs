using System;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiCode.TwentyThreeVideo.UI
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentReference), UIHint = "twentythreevideo")]
    public class TwentyThreeVideoEditorDescriptor : ContentReferenceEditorDescriptor<Video>
    {
        public override string RepositoryKey
        {
            get { return Constants.ProviderKey; }
        }
    }

    [UIDescriptorRegistration]
    public class TwentyThreeVideoUIDescriptor : UIDescriptor<Video>
    {
        public TwentyThreeVideoUIDescriptor()
        {
            ContainerTypes = new Type[] { typeof(VideoFolder) };
        }
    }

}
