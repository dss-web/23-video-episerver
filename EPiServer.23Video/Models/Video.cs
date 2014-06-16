using System;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;

namespace EPiCode.TwentyThreeVideo.Models
{
    public class Video : ContentBase, IBinaryStorable
    {
        public virtual string Id { get; set; }
        public virtual string VideoId { get; set; }
        public Blob BinaryData { get; set; }
        public Uri BinaryDataContainer { get; set; }
        [Ignore]
        public string VideoUrl { get; set; }

        [ImageDescriptor(Height = 48, Width = 48)]
        public virtual Blob Thumbnail { get; set; }
    }
}
