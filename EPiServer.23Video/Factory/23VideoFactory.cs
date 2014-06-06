using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        
        private static List<Photo> GetVideos(IApiProvider apiProvider, PhotoListParameters photoListParameters)
        {
            const string cacheKey = "PhotoService";
            //if (Caching.VideoProviderCache.Instance.InnerCache.ContainsKey(cacheKey))
            //{
            //    return Caching.VideoProviderCache.Instance.InnerCache.GetValue(cacheKey) as List<Photo>;
            //}

            List<Photo> result = new List<Photo>();
            IPhotoService photoService = new PhotoService(apiProvider);

            PhotoListParameters p = photoListParameters;
            
            if (photoListParameters == null)
            {
                p = new PhotoListParameters();
            }
            
            p.IncludeUnpublished = false;
            p.Video = true;

            List<Photo> photos = photoService.GetList(p);

            
            //Caching.VideoProviderCache.Instance.InnerCache.Add(cacheKey, result, result.Count, TimeSpan.FromMinutes(10));
            return photos;
        }

        public static void UpdateVideo(Photo photo)
        {
            IApiProvider apiProvider = _23Client.ApiProvider;

            IPhotoService service = new PhotoService(_23Client.ApiProvider);

            if (service.Update((int) photo.PhotoId, title: photo.Title) == false)
            {
                // saved failed.
            }



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
