/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;

namespace EPiCode.TwentyThreeVideo.Models
{
    [ContentType(GUID = "D2252133-FB78-470F-8122-90371EEE7A06", 
        DisplayName = "23 Video media", AvailableInEditMode = false, GroupName = "media")]
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

        public new VersionStatus Status {
            get { return base.Status; }
            set { base.Status = value;  }
        }

        [Ignore]
        public virtual bool PublishedIn23 { get; set; }
        
    }
}
