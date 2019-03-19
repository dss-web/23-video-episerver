using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visual.Domain
{
    public class PhotoUploadToken
    {
        public string UploadToken;

        public string Title;
        public string Description;
        public string Tags;

        public bool Publish;

        public int? UserId;
        public int? AlbumId;

        public int? ValidMinutes;
        public int? ValidUntil;

        public string ReturnURL;
    }

    public class PhotoBlock
    {
        public int? Width;
        public int? Height;
        public int? Size;
        public string Download;

        public PhotoBlock()
        {

        }

        public PhotoBlock(string AWidth, string AHeight, string ASize, string ADownload)
        {
            Width = (!String.IsNullOrEmpty(AWidth) ? Helpers.ConvertStringToInteger(AWidth) as int? : null);
            Height = (!String.IsNullOrEmpty(AHeight) ? Helpers.ConvertStringToInteger(AHeight) as int? : null);
            Size = (!String.IsNullOrEmpty(ASize) ? Helpers.ConvertStringToInteger(ASize) as int? : null);
            Download = ADownload;
        }
    }

    public class Photo
    {
        public int? PhotoId;

        public string Title;
        public string Token;
        public string One;

        public int? AlbumId;
        public string AlbumTitle;

        public bool? Published;

        public string CreationDateANSI;
        public string CreationDateDate;
        public string CreationDateTime;
        public string OriginalDateANSI;
        public string OriginalDateDate;
        public string OriginalDateTime;

        public int? ViewCount;
        public int? NumberOfComments;
        public int? NumberOfAlbums;
        public int? NumberOfTags;
        public int? NumberOfRatings;

        public double? PhotoRating;

        public bool? IsVideo;
        public bool? IsAudio;
        public bool? VideoEncoded;
        public bool? TextOnly;
        public bool? Promoted;

        public double? VideoLength;

        public int? UserId;
        public string Username;
        public string DisplayName;
        public string UserURL;

        public PhotoBlock Original;

        public PhotoBlock Quad16;
        public PhotoBlock Quad50;
        public PhotoBlock Quad75;
        public PhotoBlock Quad100;

        public PhotoBlock Small;
        public PhotoBlock Medium;
        public PhotoBlock Portrait;
        public PhotoBlock Standard;
        public PhotoBlock Large;

        public PhotoBlock VideoSmall;
        public PhotoBlock VideoMedium;
        public PhotoBlock VideoHD;
        public PhotoBlock Video1080p;
        public PhotoBlock VideoMobileH263AMR;
        public PhotoBlock VideoMobileH263AAC;
        public PhotoBlock VideoMobileMPEGE4AMR;
        public PhotoBlock Audio;

        public string Content;
        public string ContentText;

        public string BeforeDownloadType;
        public string BeforeDownloadURL;
        public string AfterDownloadType;
        public string AfterDownloadURL;

        public string AfterText;
        
        public string AbsoluteUrl;

        public List<string> Tags;
        public Dictionary<string, string> Variables;
    }
}
