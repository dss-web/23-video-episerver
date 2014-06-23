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
    }
}