using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;

namespace EPiCode.TwentyThreeVideo.Models
{
    public class Video : ContentBase, IBinaryStorable
    {
        [Ignore]
        public virtual string Id { get; set; }

        [ScaffoldColumn(false)]
        public Blob BinaryData { get; set; }
        [ScaffoldColumn(false)]
        public Uri BinaryDataContainer { get; set; }
        [Ignore]
        public string VideoUrl { get; set; }

        [ScaffoldColumn(false)]
        [ImageDescriptor(Height = 48, Width = 48)]
        public virtual Blob Thumbnail { get; set; }

        [Ignore]
        public virtual string oEmbedVideoName { get; set; }

        [Ignore]
        public virtual string oEmbedHtml { get; set; }
    }
}
