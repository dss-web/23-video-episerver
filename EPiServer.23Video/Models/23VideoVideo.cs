using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;

namespace EPiServer._23Video.Models
{
    public class _23VideoVideo : MediaData
    {
        public virtual string Id { get; set; }

        public virtual string VideoId { get; set; }

        /// <summary>
        /// Gets or sets the generated thumbnail for this media.
        /// 
        /// </summary>
        [ImageDescriptor(Height = 48, Width = 48)]
        public override Blob Thumbnail
        {
            get
            {
                return base.Thumbnail;
            }
            set
            {
                base.Thumbnail = value;
            }
        }
    }
}