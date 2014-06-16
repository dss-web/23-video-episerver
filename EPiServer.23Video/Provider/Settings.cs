using System;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace EPiCode.TwentyThreeVideo.Provider
{
    [EPiServerDataStore(AutomaticallyRemapStore = true, AutomaticallyCreateStore = true)]
    public class Settings
    {
        public Identity Id { get; set; }

        public string Domain { get; set; }

        public string CustomerKey { get; set; }

        public string CustomerSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public string RefreshToken { get; set; }

        public DateTime TokenCreated { get; set; }
        
        public int TokenExpires { get; set; }
    }
}