using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Visual.Utilities
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
                twentyThreeHttpWebRequest.Timeout = 7200000;  // 1 hour timeout
                return twentyThreeHttpWebRequest;
            }
        }
    }
}
