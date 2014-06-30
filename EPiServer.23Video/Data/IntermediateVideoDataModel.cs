using System;
using EPiServer.Core;
using EPiServer.Framework.Blobs;

namespace EPiCode.TwentyThreeVideo.Data
{
    public class IntermediateVideoDataModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public Guid Guid { get; set; }
        public ContentReference ContentLink { get; set; }
        public ContentReference ParentLink { get; set; }
        public Blob Thumbnail { get; set; }
        public Blob Binarydata { get; set; }
        public VideoContentType VideoContentType { get; set; }
        public string VideoUrl { get; set; }
        public string oEmbedHtml { get; set; }
        public string oEmbedVideoName { get; set; }
    }
}