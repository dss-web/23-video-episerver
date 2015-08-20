/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */

using System.Collections.Generic;
using EPiServer.Data;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public class Settings
    {
        public Identity Id { get; set; }
        public string Enabled { get; set; }
        public string Domain { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public bool OEmbedIsEnabled { get; set; }
        public IDictionary<string, string> EditorGroupChannelMapping { get; set; } 
    }
}