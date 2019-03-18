/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Castle.Core.Internal;
using EPiCode.TwentyThreeVideo.Models;
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer.Cms.Shell.UI.Controllers.Preview;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.DataAnnotations;
using EPiServer.Framework.Blobs;
using EPiServer.ServiceLocation;
using log4net;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo
{
    public class VideoHelper
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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

        public bool PopulateVideo(Video video, Photo item)
        {
            if (item.PhotoId != null)
            {
                try
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
                    video.VideoDownloadUrl = GetVideoDownloadUrl(item.VideoHD);
                    video.PublishedIn23 = item.Published ?? false;
                    if (SettingsRepository.Service.oEmbedIsEnabled && item.Published == true)
                    {
                        var oEmbedCode = TwentyThreeVideoRepository.GetoEmbedCodeForVideo(item.One);
                        if (string.IsNullOrWhiteSpace(oEmbedCode))
                        {
                            _log.InfoFormat(
                                    "23Video: 23Video returned empty oembed code. Videoname from 23Video {0}",
                                        item.One);
                            return false;
                        }
                        video.oEmbedHtml = TwentyThreeVideoRepository.GetoEmbedCodeForVideo(item.One);
                    }
                    PopulateStandardVideoProperties(video);
                }
                catch (Exception e)
                {
                    _log.ErrorFormat("VideoHelper: PopulateVideo failed: VideoID: {0}, PhotoID: {1}, Exception: {2}", video != null && video.Id != null ? video.Id : "null", item != null && item.PhotoId != null ? item.PhotoId : 0, e.Message);
                    throw;
                }
               
            }
            return true;
        }

        public static string EmbedCode(string photoId, string photoToken, int? width, int? height)
        {
            string domain = Client.Settings.Domain;
            string widthString = width.ToString();
            string heightString = (height == null ? Math.Round(width.Value / 16.0 * 9.0).ToString() : height.Value.ToString());
            return "<iframe src=\"http://" + domain + "/v.ihtml?token=" + photoToken + "&photo%5fid=" + photoId + "\" width=\"" + widthString + "\" height=\"" + heightString + "\" frameborder=\"0\" border=\"0\" scrolling=\"no\"></iframe>";
        }

        private string GetVideoDownloadUrl(PhotoBlock photoblock)
        {
            if (photoblock == null || photoblock.Download.IsNullOrEmpty())
                return string.Empty;
            var domain = Client.Settings.Domain;
            var url = string.Format("http://{0}{1}", domain, photoblock.Download);
            return url;
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
            var blob = ServiceLocator.Current.GetInstance<IBlobFactory>().GetBlob(new Uri(string.Format("{0}://{1}/{2}/{3}", Blob.BlobUriScheme, Blob.DefaultProvider, container, "original.jpg")));
            using (var stream = new MemoryStream(imageData))
            {
                blob.Write(stream);
            }
            return blob;
        }

    }
}
