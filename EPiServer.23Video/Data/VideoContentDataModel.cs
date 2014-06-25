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