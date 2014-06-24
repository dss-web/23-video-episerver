using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EPiCode.TwentyThreeVideo.Models;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo
{
    public class VideoHelper
    {
        protected Injected<ThumbnailManager> ThumbnailManager { get; set; }
        protected Injected<SettingsRepository> SettingsRepository { get; set; }

        public Video PopulateStandardVideoProperties(Video video)
        {
            video.Created = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
            video.Changed = video.Created;
            video.IsPendingPublish = false;
            video.StartPublish = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
            video.Status = VersionStatus.Published;
            video.MakeReadOnly();
            return video;
        }

        public Video PopulateVideo(Video video, Photo item)
        {
            if (item.PhotoId != null)
            {
                int id = (int)(item.PhotoId);
                video.VideoUrl = EmbedCode(item.PhotoId.ToString(), item.Token, item.VideoHD.Width ?? item.VideoMedium.Width ?? item.VideoSmall.Width, item.VideoHD.Height ?? item.VideoMedium.Height ?? item.VideoSmall.Height);
                video.ContentLink = new ContentReference((id).GetHashCode(), 0, Constants.ProviderKey);
                video.Id = id.ToString();
                video.ContentGuid = StringToGuid(id.ToString());
                video.Name = item.Title;
                video.BinaryData = GetThumbnail(item);
                video.Thumbnail = ThumbnailManager.Service.CreateImageBlob(video.BinaryData, "thumbnail", new ImageDescriptorAttribute(48, 48));
                video.oEmbedVideoName = item.One;
                if (SettingsRepository.Service.oEmbedIsEnabled)
                {
                    video.oEmbedHtml = TwentyThreeVideoRepository.GetoEmbedCodeForVideo(item.One);
                }
                PopulateStandardVideoProperties(video);
            }
            return video;
        }

        public static string EmbedCode(string photoId, string photoToken, int? width, int? height)
        {
            string domain = Client.Settings.Domain;
            string widthString = width.ToString();
            string heightString = (height == null ? Math.Round(width.Value / 16.0 * 9.0).ToString() : height.Value.ToString());
            return "<iframe src=\"http://" + domain + "/v.ihtml?token=" + photoToken + "&photo%5fid=" + photoId + "\" width=\"" + widthString + "\" height=\"" + heightString + "\" frameborder=\"0\" border=\"0\" scrolling=\"no\"></iframe>";
        }

        private static Guid StringToGuid(string value)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            return new Guid(data);
        }

        private Blob GetThumbnail(Photo item)
        {
            var webClient = new WebClient();
            var url = "http://" + Client.Settings.Domain + item.Original.Download;
            var imageData = webClient.DownloadData(url);
            string container = item.PhotoId.ToString();
            var blob = BlobFactory.Instance.GetBlob(new Uri(string.Format("{0}://{1}/{2}/{3}", Blob.BlobUriScheme, Blob.DefaultProvider, container, "original.jpg")));
            using (var stream = new MemoryStream(imageData))
            {
                blob.Write(stream);
            }
            return blob;
        }

    }
}
