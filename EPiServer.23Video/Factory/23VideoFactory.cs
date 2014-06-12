using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Visual;
using Visual.Domain;

namespace EPiServer._23Video.Factory
{
    public static class _23VideoFactory
    {
        public static List<Photo> GetVideoList()
        {
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Photo> photos = GetVideos(apiProvider, null);

            return photos;

        }

        public static List<Photo> GetVideoList(int channelId)
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Photo> photos = GetVideos(apiProvider, new PhotoListParameters(){AlbumId = channelId});

            return photos;

        }

        public static Photo GetVideo(int videoId)
        {
            IPhotoService photoService = new PhotoService(_23Client.ApiProvider);
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

            PhotoListParameters p = photoListParameters;
            
            if (photoListParameters == null)
            {
                p = new PhotoListParameters();
            }
            
            p.IncludeUnpublished = true;
            p.Video = true;

            List<Photo> photos = photoService.GetList(p);

            
            //Caching.VideoProviderCache.Instance.InnerCache.Add(cacheKey, result, result.Count, TimeSpan.FromMinutes(10));
            return photos;
        }

        public static void UpdateVideo(Photo photo)
        {
            IPhotoService service = new PhotoService(_23Client.ApiProvider);

            if (service.Update((int) photo.PhotoId, title: photo.Title) == false)
            {
                // saved failed.
            }
        }

        public static int? UploadVideo(string filename, Stream stream, int channel)
        {
            IPhotoService service = new PhotoService(_23Client.ApiProvider);

            string fileExtention = Path.GetExtension(filename);

            if (fileExtention == null)
            {
                //TODO: Choose a default extention if missing.
                //default extention if missing
                fileExtention = ".mp4";
            }  

            return service.Upload(filename: filename, fileContentType: fileExtention.TrimStart('.'), filestream: stream, albumId:channel, title:filename);
        }

        public static List<Album> GetChannelList()
        {
            // Check that we actually have everything configured
            IApiProvider apiProvider = _23Client.ApiProvider;

            // Get a list of videos to throw in there
            List<Album> albums = GetChannels(apiProvider);

            return albums;

        }

        private static List<Album> GetChannels(IApiProvider apiProvider)
        {
            IAlbumService albumService = new AlbumService(apiProvider);

            return albumService.GetList();
        }

    }
}
