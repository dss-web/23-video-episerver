/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
namespace EPiCode.TwentyThreeVideo.oEmbed
{
    public class oEmbedResponse
    {
        public string version { get; set; }
        public string type { get; set; }
        public string photo_id { get; set; }
        public string album_id { get; set; }
        public string tag { get; set; }
        public string title { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        public string cache_age { get; set; }
        public string provider_name { get; set; }
        public string provider_url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string html { get; set; }
        
        public string RenderMarkup()
        {
            return string.IsNullOrEmpty(this.html) ? string.Empty : this.html;
        }
    }
}