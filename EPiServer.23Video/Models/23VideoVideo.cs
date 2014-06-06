using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Web.Routing;

namespace EPiServer._23Video.Models
{
    public class _23VideoVideo : ContentBase, IBinaryStorable, IRoutable
    {
        public virtual string Id { get; set; }
        public virtual string VideoId { get; set; }
        public Blob BinaryData { get; set; }
        public Uri BinaryDataContainer { get; private set; } 

        [ImageDescriptor(Height = 48, Width = 48)]
        public virtual Blob Thumbnail { get; set; }

        [UIHint("previewabletext")]
        public virtual string URLSegment { get; set; }

        public string RouteSegment
        {
            get { return URLSegment; }
            set { URLSegment = value; }
        }
    }
}