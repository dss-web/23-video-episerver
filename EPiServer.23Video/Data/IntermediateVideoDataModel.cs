/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
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
        public string VideoDownloadUrl { get; set; }
        public string EditorGroup { get; set; }
    }
}