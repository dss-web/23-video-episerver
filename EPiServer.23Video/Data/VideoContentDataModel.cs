using System;
using EPiServer.Data;

namespace EPiCode.TwentyThreeVideo.Data
{
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