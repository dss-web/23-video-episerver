using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;

namespace Visual
{
    public class AlbumService : IAlbumService
    {
        private IApiProvider _provider;

        public AlbumService(IApiProvider provider)
        {
            _provider = provider;
        }

        // * Get a single album
        /// <summary>
        /// Returns a single album not depending on whether it's hidden or not
        /// </summary>
        public Domain.Album Get(int albumId)
        {
            // Get a list of albums
            List<Domain.Album> albumList = GetList(new AlbumListParameters
            {
                AlbumId = albumId,
                IncludeHidden = true
            });

            // Verify album list
            if ((albumList == null) || (albumList.Count == 0)) return null;

            // Return the first entry
            return albumList[0];
        }

        // * Get album list
        // Implements http://www.23developer.com/api/album-list
        /// <summary>
        /// Returns a list of albums by default parameters
        /// </summary>
        public List<Domain.Album> GetList()
        {
            return GetList(new AlbumListParameters());
        }

        /// <summary>
        /// Returns a list of albums by specific parameters
        /// </summary>
        public List<Domain.Album> GetList(AlbumListParameters requestParameters)
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            if (requestParameters.AlbumId != null) requestUrlParameters.Add("album_id=" + requestParameters.AlbumId);
            if (requestParameters.UserId != null) requestUrlParameters.Add("user_id=" + requestParameters.UserId);
            if (requestParameters.PhotoId != null) requestUrlParameters.Add("photo_id=" + requestParameters.PhotoId);

            if (requestParameters.Search != null) requestUrlParameters.Add("search=" + HttpUtility.UrlEncode(requestParameters.Search));

            if (requestParameters.IncludeHidden) requestUrlParameters.Add("include_hidden_p=1");

            if (requestParameters.OrderBy != AlbumListSort.CreationDate) requestUrlParameters.Add("orderby=" + RequestValues.Get(requestParameters.OrderBy));
            if (requestParameters.Order != GenericSort.Descending) requestUrlParameters.Add("order=" + RequestValues.Get(requestParameters.Order));

            if (requestParameters.PageOffset != null) requestUrlParameters.Add("p=" + requestParameters.PageOffset);
            if (requestParameters.Size != null) requestUrlParameters.Add("size=" + requestParameters.Size);

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/album/list", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // List all the videos
            XPathNodeIterator albums = responseMessage.Select("/response/album");
            List<Domain.Album> result = new List<Domain.Album>();

            while (albums.MoveNext())
            {
                if (albums.Current == null) return null;

                // Create the domain album
                Domain.Album albumModel = new Domain.Album
                                              {
                                                  AlbumId = Helpers.ConvertStringToInteger(albums.Current.GetAttribute("album_id", "")),

                                                  Title = albums.Current.GetAttribute("title", ""),

                                                  PrettyDate = albums.Current.GetAttribute("pretty_date", ""),
                                                  PrettyTime = albums.Current.GetAttribute("pretty_time", ""),

                                                  One = albums.Current.GetAttribute("one", ""),
                                                  CreationDateANSI = albums.Current.GetAttribute("creation_date_ansi", ""),

                                                  UserId = Helpers.ConvertStringToInteger(albums.Current.GetAttribute("user_id", "")),
                                                  UserUrl = albums.Current.GetAttribute("user_url", ""),
                                                  Username = albums.Current.GetAttribute("username", ""),
                                                  DisplayName = albums.Current.GetAttribute("display_name", ""),

                                                  Token = albums.Current.GetAttribute("token", ""),
                                                  Hide = (albums.Current.GetAttribute("hide_p", "") == "1"),

                                                  Content = Helpers.GetNodeChildValue(albums.Current, "content"),
                                                  ContentText = Helpers.GetNodeChildValue(albums.Current, "content_text")
                                              };

                result.Add(albumModel);
            }
            
            return result;
        }

        // * Create album
        // Implements http://www.23developer.com/api/album-create
        public int? Create(string title, string description = "", bool hide = false, int? userId = null)
        {
            // Verify required parameters
            if (String.IsNullOrEmpty(title)) return null;

            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            requestUrlParameters.Add("title=" + HttpUtility.UrlEncode(title));
            if (!String.IsNullOrEmpty(description)) requestUrlParameters.Add("description=" + HttpUtility.UrlEncode(description));
            if (hide) requestUrlParameters.Add("hide_p=1");
            if (userId != null) requestUrlParameters.Add("user_id=" + userId);

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/album/create", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // Get the album id
            XPathNodeIterator albums = responseMessage.Select("/response/album_id");
            if ((albums.MoveNext()) && (albums.Current != null)) return Helpers.ConvertStringToInteger(albums.Current.Value);
            
            // If nothing pops up, we'll return null
            return null;
        }

        // * Update album
        // Implements http://www.23developer.com/api/album-update
        public bool Update(int albumId, string title, string description = "", bool hide = false)
        {
            // Verify required parameters
            if (String.IsNullOrEmpty(title)) return false;

            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            requestUrlParameters.Add("album_id=" + albumId);
            requestUrlParameters.Add("title=" + HttpUtility.UrlEncode(title));
            if (!String.IsNullOrEmpty(description)) requestUrlParameters.Add("description=" + HttpUtility.UrlEncode(description));
            if (hide) requestUrlParameters.Add("hide_p=1");

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/album/update", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return false;

            // If nothing pops up, we'll return true
            return true;
        }

        // * Delete album
        // Implements http://www.23developer.com/api/album-delete
        /// <summary>Delete an album given an id</summary>
        public bool Delete(int albumId)
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            requestUrlParameters.Add("album_id=" + albumId);

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/album/delete", requestUrlParameters), HttpDeliveryMethods.PostRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return false;

            // If nothing pops up, we'll return true
            return true;
        }
    }
}
