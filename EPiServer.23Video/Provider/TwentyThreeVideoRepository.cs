/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using EPiCode.TwentyThreeVideo.oEmbed;
using EPiServer.Framework.Blobs;
using EPiServer.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Visual;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public static class TwentyThreeVideoRepository
    {
        private static readonly ILogger _log = LogManager.GetLogger();

        public static List<Photo> GetVideoList()
        {
            IApiProvider apiProvider = Client.ApiProvider;
            // Get a list of videos to throw in there
            List<Photo> photos = GetVideos(apiProvider, null);
            return photos;
        }

        public static List<Photo> GetVideoList(int channelId)
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Photo> photos = GetVideos(apiProvider, new PhotoListParameters() { AlbumId = channelId });
            return photos;
        }

        public static Photo GetVideo(int videoId)
        {
            IPhotoService photoService = new PhotoService(Client.ApiProvider);
            return photoService.Get(videoId, true);
        }

        private static List<Photo> GetVideos(IApiProvider apiProvider, PhotoListParameters photoListParameters)
        {
            var photoService = new PhotoService(apiProvider);
            var p = photoListParameters;

            if (photoListParameters == null)
            {
                p = new PhotoListParameters();
            }

            p.IncludeUnpublished = false;
            p.PageOffset = 1;
            p.Size = 20;
            p.Video = true;

            var allPhotos = new List<Photo>();
            var completed = false;
            while (!completed)
            {
                var photos = photoService.GetList(p);
                allPhotos.AddRange(photos);
                completed = photos.Count < p.Size;
                p.PageOffset++;
            }
            return allPhotos;
        }

        public static void UpdateVideo(Photo photo)
        {
            IPhotoService service = new PhotoService(Client.ApiProvider);

            if (service.Update((int)photo.PhotoId, title: photo.Title) == false)
            {
                _log.Error("23Video: Failed updating video with id {0}, title: {1}", photo.PhotoId, photo.Title);
            }
        }

        public static int? UploadVideo(string filename, Blob blob, int channel, int? userId = null)
        {
            string fileExtention = Path.GetExtension(filename);
            var title = Path.GetFileNameWithoutExtension(filename);
            if (fileExtention == null)
            {
                // default extention if missing
                fileExtention = ".mp4";//aosrei
            }
            try
            {
                IPhotoService service = new PhotoService(Client.ApiProvider);
                using (var stream = blob.OpenRead() as FileStream)
                {
                    return service.Upload(filename, fileExtention.TrimStart('.'), stream, albumId: channel, title: title, userId: userId);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error uploading video to 23 Video:", ex);
                throw ex;
            }
        }

        public static List<Album> GetChannelList()
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Album> albums = GetChannels(apiProvider);
            return albums;
        }

        public static List<User> SearchUsers(string term)
        {
            var service = new UserService(Client.ApiProvider);
            return service.GetList(new UserListParameters
            {
                Search = term
            });
        }

        private static List<Album> GetChannels(IApiProvider apiProvider)
        {
            IAlbumService albumService = new AlbumService(apiProvider);
            return albumService.GetList();
        }

        public static string GetoEmbedCodeForVideo(string videoName)
        {
            string jsonResponse = string.Empty;
            string endpoint = string.Format("http://{0}/oembed?format=json&url=http://{0}{1}", Client.Settings.Domain, videoName);
            var webClient = new WebClient();
            try
            {
                jsonResponse = webClient.DownloadString(endpoint);

                if (!string.IsNullOrEmpty(jsonResponse))
                {
                    var jSerialize = new JavaScriptSerializer();
                    var oEmbedResponse = jSerialize.Deserialize<oEmbedResponse>(jsonResponse);
                    return oEmbedResponse.RenderMarkup();
                }
            }
            catch (Exception exception)
            {
                _log.Warning("23Video: Error getting oEmbed code for {0}, endpoint {1}. Exception was {2}", videoName, endpoint, exception);
            }
            return string.Empty;
        }
    }
}