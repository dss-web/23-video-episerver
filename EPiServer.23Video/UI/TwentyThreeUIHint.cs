using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiCode.TwentyThreeVideo.Models;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiCode.TwentyThreeVideo.UI
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentReference), UIHint = "twentythreevideo")]
    public class BlockReferenceEditorDescriptor : ContentReferenceEditorDescriptor<Video>
    {
        public override IEnumerable<ContentReference> Roots
        {
            get
            {
                return new ContentReference[] { new ContentReference(164) };
            }
        }
    }

}
