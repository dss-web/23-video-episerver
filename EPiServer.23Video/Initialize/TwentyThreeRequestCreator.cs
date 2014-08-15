using System;
using System.Net;

namespace EPiCode.TwentyThreeVideo.Initialize
{
    public static class TwentyThreeRequestCreator
    {
        private static IWebRequestCreate _twentyThreeCreator;
        public static IWebRequestCreate TwentyThreeHttp
        {
            get
            {
                if (_twentyThreeCreator == null)
                {
                    _twentyThreeCreator = new TwentyThreeHttpRequestCreator();
                }
                return _twentyThreeCreator;
            }
        }

        private class TwentyThreeHttpRequestCreator : IWebRequestCreate
        {
            public WebRequest Create(Uri uri)
            {       
                    var twentyThreeHttpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(uri);
                    twentyThreeHttpWebRequest.Timeout = 3600000;
                    return twentyThreeHttpWebRequest;
            }
        }
    }

}
