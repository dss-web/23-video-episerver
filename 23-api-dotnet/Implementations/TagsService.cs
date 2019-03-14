using System.Collections.Generic;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;
using System.Web;

namespace Visual
{
    public class TagService : ITagService
    {
        private IApiProvider _provider;

        public TagService(IApiProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Get a list of tags defined by the default request parameters
        /// </summary>
        /// <returns></returns>
        public List<Domain.Tag> GetList()
        {
            return GetList(new TagListParameters());
        }

        /// <summary>
        /// Get a list of tags defined by the request parameters
        /// </summary>
        /// <param name="requestParameters"></param>
        /// <returns></returns>
        public List<Domain.Tag> GetList(TagListParameters requestParameters)
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            if (requestParameters.Search != null) requestUrlParameters.Add("search=" + HttpUtility.UrlEncode(requestParameters.Search));

            requestUrlParameters.Add("reformat_tags_p=" + (requestParameters.ReformatTags ? 1 : 0));
            requestUrlParameters.Add("exclude_machine_tags_p=" + (requestParameters.ExcludeMachineTags ? 1 : 0));

            if (requestParameters.OrderBy != TagListSort.Tag) requestUrlParameters.Add("orderby=" + RequestValues.Get(requestParameters.OrderBy));
            if (requestParameters.Order != GenericSort.Descending) requestUrlParameters.Add("order=" + RequestValues.Get(requestParameters.Order));

            if (requestParameters.PageOffset != null) requestUrlParameters.Add("p=" + requestParameters.PageOffset);
            if (requestParameters.Size != null) requestUrlParameters.Add("size=" + requestParameters.Size);

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/tag/list", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // List all the videos
            XPathNodeIterator tags = responseMessage.Select("/response/tag");
            List<Domain.Tag> result = new List<Domain.Tag>();

            while (tags.MoveNext())
            {
                if (tags.Current == null) return null;

                // Create the domain Tag
                Domain.Tag tagModel = new Domain.Tag
                                          {
                                              Name = tags.Current.GetAttribute("tag", ""),
                                              URL = tags.Current.GetAttribute("url", ""),
                                              Count = Helpers.ConvertStringToInteger(tags.Current.GetAttribute("count", ""))
                                          };

                result.Add(tagModel);
            }

            return result;
        }

        public List<Domain.Tag> GetRelatedList(string name)
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            requestUrlParameters.Add("tag=" + HttpUtility.UrlEncode(name));

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/tag/related", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // List all the videos
            XPathNodeIterator tags = responseMessage.Select("/response/relatedtag");
            List<Domain.Tag> result = new List<Domain.Tag>();

            while (tags.MoveNext())
            {
                if (tags.Current == null) return null;

                // Create the domain Tag
                Domain.Tag tagModel = new Domain.Tag
                                          {
                                              Name = tags.Current.GetAttribute("tag", ""),
                                              URL = tags.Current.GetAttribute("url", ""),
                                              Count = null
                                          };

                result.Add(tagModel);
            }

            return result;
        }
    }
}
