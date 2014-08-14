using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.WebPages.Scope;
using DotNetOpenAuth.Messaging;
using EPiCode.TwentyThreeVideo.Data;
using EPiCode.TwentyThreeVideo.oEmbed;
using EPiServer.Framework.Blobs;
using log4net;
using Visual;
using Visual.Domain;

namespace EPiCode.TwentyThreeVideo.Provider
{
    public static class TwentyThreeVideoRepository
    {
        private static ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            const string cacheKey = "PhotoService";
            //if (Caching.VideoProviderCache.Instance.InnerCache.ContainsKey(cacheKey))
            //{
            //    return Caching.VideoProviderCache.Instance.InnerCache.GetValue(cacheKey) as List<Photo>;
            //}

            IPhotoService photoService = new PhotoService(apiProvider);

            Visual.SiteService vc = new SiteService(apiProvider);
            Visual.ISessionService ss = new SessionService(apiProvider);

            PhotoListParameters p = photoListParameters;

            if (photoListParameters == null)
            {
                p = new PhotoListParameters();
            }

            p.IncludeUnpublished = true;
            p.Video = true;

            List<Photo> photos = photoService.GetList(p);
            //Caching.VideoProviderCache.Instance.InnerCache.Update(cacheKey, result, result.Count, TimeSpan.FromMinutes(10));
            return photos;
        }

        public static void UpdateVideo(Photo photo)
        {
            IPhotoService service = new PhotoService(Client.ApiProvider);

            if (service.Update((int)photo.PhotoId, title: photo.Title) == false)
            {
                _log.ErrorFormat("23Video: Failed updating video with id {0}, title: {1}", photo.PhotoId, photo.Title);
            }
        }

        public static int? UploadVideo(string filename, Blob stream, int channel)
        {
            // IPhotoService service = new PhotoService(Client.ApiProvider);
            string fileExtention = Path.GetExtension(filename);

            if (fileExtention == null)
            {
                //TODO: Choose a default extention if file has no extention.
                //default extention if missing
                fileExtention = ".mp4";//aosrei
            }
            //var token = service.GetUploadToken(" ", false, null, channel, filename, "", "", true, 180, 1);
            //service.RedeemUploadToken(filename, fileExtention, stream, token.UploadToken);
            //var uploadToken = token.UploadToken;
            // return token.UploadToken
            //int num = filename.LastIndexOf('\\');
            //var _provider = Client.ApiProvider;
            //var xPathNavigator = _provider.DoRequest(new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/photo/redeem-upload-token", (List<string>)null), HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.PostRequest), new List<MultipartPostPart>()
            //  {
            //    MultipartPostPart.CreateFormFilePart("file", num == -1 ? filename : filename.Substring(num + 1), fileExtention, stream),
            //    MultipartPostPart.CreateFormPart("upload_token", uploadToken)
            //  });
            var worker = new BackgroundWorker();
            worker.DoWork += (obj, e) => FileUpload(filename, fileExtention.TrimStart('.'), stream, channel, filename);
            worker.RunWorkerAsync();



            return 0;
            // return service.Upload(filename, fileExtention.TrimStart('.'), stream,channel, title: filename);
        }

        private static int? FileUpload(string filename, string fileExtention, Blob blob, int channel, string title)
        {
       
            IPhotoService service = new PhotoService(Client.ApiProvider);
            var token = service.GetUploadToken(" ", false, null, channel, filename, "", "", true, 180, 5);
            using (FileStream stream = blob.OpenRead() as FileStream)
            {
                token.ValidUntil = int.MaxValue;
                service.RedeemUploadToken(filename, fileExtention, stream, token.UploadToken);
                // return service.Upload(filename, fileExtention.TrimStart('.'), stream, channel, title: title);
            }

            VideoSynchronizationEventHandler.DataStoreUpdated();
            return 0;
        }

        public static List<Album> GetChannelList()
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Album> albums = GetChannels(apiProvider);

            return albums;

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
            }
            catch (WebException exception)
            {
                _log.ErrorFormat("23Video: Error getting oEmbed code for {0}, endpoint {1}. Exception was {2}", videoName, endpoint, exception);
            }

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                var jSerialize = new JavaScriptSerializer();
                var oEmbedResponse = jSerialize.Deserialize<oEmbedResponse>(jsonResponse);
                return oEmbedResponse.RenderMarkup();
            }

            return string.Empty;
        }
    }
}
