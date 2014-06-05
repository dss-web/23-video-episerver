using System;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace EPiServer._23Video.Initialize
{
    [EPiServerDataStore(AutomaticallyRemapStore = true, AutomaticallyCreateStore = true)]
    public class _23VideoSettings
    {
        public Identity Id { get; set; }

        public string CustomerKey { get; set; }

        public string CustomerSecret { get; set; }

        public string AccessToken { get; set; }

        public string AccessTokenSecret { get; set; }

        public string RefreshToken { get; set; }

        public DateTime TokenCreated { get; set; }
        
        public int TokenExpires { get; set; }
    }
}