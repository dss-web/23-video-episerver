/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace EPiCode.TwentyThreeVideo.Data
{
    [EPiServerDataStore(AutomaticallyCreateStore = true)]
    public class VideoContentDataModel
    {
        public Identity Id
        {
            get { return new Guid(Constants.VideoContentDataModel); }
        }

        public string SerializedContent { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}