using System;
using System.Collections.Generic;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiCode.TwentyThreeVideo.UI
{
    [EditorDescriptorRegistration(TargetType = typeof(PageReference))]
    public class VideoEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(
           ExtendedMetadata metadata,
           IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            dynamic mayQuack = metadata;
            var ownerContent = mayQuack.OwnerContent as IContent;
            if (ownerContent is Video)
            {
                
                metadata.ShowForEdit = false;
            }  
        }

    }
}
