using System;
using System.Net;

namespace EPiCode.TwentyThreeVideo.Initialize
{
    public static class TwentyThreeCreatorRequestCreator
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
                    twentyThreeHttpWebRequest.Timeout = 2000000;
                    return twentyThreeHttpWebRequest;
            }
        }
    }

}
