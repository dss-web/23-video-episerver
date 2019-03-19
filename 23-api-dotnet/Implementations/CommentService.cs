using System.Collections.Generic;
using System.Web;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;
using System;

namespace Visual
{
    public class CommentService : ICommentService
    {
        private IApiProvider _provider;

        public CommentService(IApiProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Returns a list of comments by default parameters
        /// </summary>
        public List<Domain.Comment> GetList()
        {
            return GetList(new CommentListParameters());
        }

        /// <summary>
        /// Returns a list of comments by specific parameters
        /// </summary>
        public List<Domain.Comment> GetList(CommentListParameters requestParameters)
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            if (requestParameters.ObjectId != null) requestUrlParameters.Add("object_id=" + requestParameters.ObjectId);
            if (requestParameters.ObjectType != CommentObjectType.Empty) requestUrlParameters.Add("order=" + RequestValues.Get(requestParameters.ObjectType));

            if (requestParameters.CommentId != null) requestUrlParameters.Add("comment_id=" + requestParameters.CommentId);
            if (requestParameters.CommentUserId != null) requestUrlParameters.Add("comment_user_id=" + requestParameters.CommentUserId);

            if (requestParameters.Search != null) requestUrlParameters.Add("search=" + HttpUtility.UrlEncode(requestParameters.Search));

            if (requestParameters.Order != GenericSort.Descending) requestUrlParameters.Add("order=" + RequestValues.Get(requestParameters.Order));

            if (requestParameters.PageOffset != null) requestUrlParameters.Add("p=" + requestParameters.PageOffset);
            if (requestParameters.Size != null) requestUrlParameters.Add("size=" + requestParameters.Size);

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/comment/list", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // List all the videos
            XPathNodeIterator comments = responseMessage.Select("/response/comment");
            List<Domain.Comment> result = new List<Domain.Comment>();

            while (comments.MoveNext())
            {
                if (comments.Current == null) return null;
                
                // Create the domain album
                Domain.Comment commentModel = new Domain.Comment();

                commentModel.CommentId = Helpers.ConvertStringToInteger(comments.Current.GetAttribute("comment_id", ""));

                commentModel.ObjectId = Helpers.ConvertStringToInteger(comments.Current.GetAttribute("object_id", ""));
                switch (comments.Current.GetAttribute("object_type", ""))
                {
                    case "album":
                        commentModel.ObjectType = CommentObjectType.Album;
                        break;

                    case "photo":
                        commentModel.ObjectType = CommentObjectType.Photo;
                        break;

                    default:
                        commentModel.ObjectType = CommentObjectType.Empty;
                        break;
                }

                commentModel.PrettyDate = comments.Current.GetAttribute("pretty_date", "");
                commentModel.PrettyTime = comments.Current.GetAttribute("pretty_time", "");

                commentModel.ShortDate = comments.Current.GetAttribute("short_date", "");
                commentModel.CreationDateANSI = comments.Current.GetAttribute("creation_date_ansi", "");

                commentModel.UserId = (!String.IsNullOrEmpty(comments.Current.GetAttribute("user_id", "")) ? (int?)Helpers.ConvertStringToInteger(comments.Current.GetAttribute("user_id", "")) : null);
                commentModel.Name = comments.Current.GetAttribute("name", "");
                commentModel.Email = comments.Current.GetAttribute("email", "");
                commentModel.TruncatedName = comments.Current.GetAttribute("truncated_name", "");

                commentModel.Content = Helpers.GetNodeChildValue(comments.Current, "content");
                commentModel.ContentText = Helpers.GetNodeChildValue(comments.Current, "content_text");

                result.Add(commentModel);
            }

            return result;
        }
    }
}
