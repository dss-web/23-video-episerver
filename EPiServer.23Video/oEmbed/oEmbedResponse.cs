using System.Web;

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