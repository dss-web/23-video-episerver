using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EPiServer.Core;
using EPiServer.Core.Transfer;
using EPiServer.DataAccess;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.Web.Routing;

namespace EPiServer._23Video.Models
{
    public class _23VideoVideo : ContentBase, IBinaryStorable
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