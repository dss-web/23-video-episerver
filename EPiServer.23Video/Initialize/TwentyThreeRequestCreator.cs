﻿/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
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
