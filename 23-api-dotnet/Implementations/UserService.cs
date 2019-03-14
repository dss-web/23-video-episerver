using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;
using Visual.Exceptions;

namespace Visual
{
    public class UserService : IUserService
    {
        private IApiProvider _provider;

        public UserService(IApiProvider provider)
        {
            _provider = provider;
        }

        // * Get user
        public Domain.User Get(int userId)
        {
            // Get a list of users
            List<Domain.User> userList = GetList(new UserListParameters
                                                     {
                                                         UserId = userId
                                                     });

            // Verify user list
            if ((userList == null) || (userList.Count == 0)) return null;

            // Return the first entry
            return userList[0];
        }

        // * Get user list
        // Implements http://www.23developer.com/api/user-list
        /// <summary>
        /// Returns a list of users by default parameters
        /// </summary>
        public List<Domain.User> GetList()
        {
            return GetList(new UserListParameters());
        }

        /// <summary>
        /// Returns a list of users by specific parameters
        /// </summary>
        public List<Domain.User> GetList(UserListParameters requestParameters)
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            if (requestParameters.UserId != null) requestUrlParameters.Add("user_id=" + requestParameters.UserId);

            if (requestParameters.Search != null) requestUrlParameters.Add("search=" + HttpUtility.UrlEncode(requestParameters.Search));

            if (requestParameters.OrderBy != UserListSort.DisplayName) requestUrlParameters.Add("orderby=" + RequestValues.Get(requestParameters.OrderBy));
            if (requestParameters.Order != GenericSort.Descending) requestUrlParameters.Add("order=" + RequestValues.Get(requestParameters.Order));

            if (requestParameters.PageOffset != null) requestUrlParameters.Add("p=" + requestParameters.PageOffset);
            if (requestParameters.Size != null) requestUrlParameters.Add("size=" + requestParameters.Size);

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/user/list", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // List all the videos
            XPathNodeIterator users = responseMessage.Select("/response/user");
            List<Domain.User> result = new List<Domain.User>();

            while (users.MoveNext())
            {
                if (users.Current == null) return null;

                // Create the domain User
                Domain.User userModel = new Domain.User
                                            {
                                                UserId = Helpers.ConvertStringToInteger(users.Current.GetAttribute("user_id", "")),
                                                Username = users.Current.GetAttribute("username", ""),
                                                DisplayName = users.Current.GetAttribute("display_name", ""),
                                                URL = users.Current.GetAttribute("url", ""),
                                                FullName = users.Current.GetAttribute("full_name", ""),
                                                Email = users.Current.GetAttribute("email", ""),
                                                SiteAdmin = (users.Current.GetAttribute("site_admin", "") != "f"),
                                                AboutAbstract = Helpers.GetNodeChildValue(users.Current, "about_abstract")
                                            };

                result.Add(userModel);
            }
            
            return result;
        }

        // * Create user
        // Implements http://www.23developer.com/api/user-create
        /// <summary>Create a user specified by an e-mail address, username, password, full name, timezone and site admin rigts specification</summary>
        public int? Create(string email, string username = null, string password = null, string fullName = null, Timezone timezone = Timezone.CET, bool siteAdmin = false)
        {
            // Verify required parameters
            if (String.IsNullOrEmpty(email)) return null;

            // Build request URL
            List<string> requestUrlParameters = new List<string>();
            
            requestUrlParameters.Add("email=" + HttpUtility.UrlEncode(email));
            if (!String.IsNullOrEmpty(username)) requestUrlParameters.Add("username=" + HttpUtility.UrlEncode(username));
            if (!String.IsNullOrEmpty(password)) requestUrlParameters.Add("password=" + HttpUtility.UrlEncode(password));
            if (!String.IsNullOrEmpty(fullName)) requestUrlParameters.Add("full_name=" + HttpUtility.UrlEncode(fullName));
            requestUrlParameters.Add("timezone=" + HttpUtility.UrlEncode(RequestValues.Get(timezone)));
            if (siteAdmin) requestUrlParameters.Add("site_admin=1");

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/user/create", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // Get the User id
            XPathNodeIterator users = responseMessage.Select("/response/user_id");
            if ((users.MoveNext()) && (users.Current != null)) return Helpers.ConvertStringToInteger(users.Current.Value);
            
            // If nothing pops up, we'll return null
            return null;
        }
		
		public Domain.Session GetLoginToken(int userId, string returnUrl = "/")
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();
			
			requestUrlParameters.Add("user_id=" + HttpUtility.UrlEncode(userId.ToString()));
            requestUrlParameters.Add("return_url=" + HttpUtility.UrlEncode(returnUrl));

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/user/get-login-token", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // Create result
            Domain.Session result = new Domain.Session();

            // Get the access token
            XPathNodeIterator accessTokenNode = responseMessage.Select("/response/login_token");
            if ((!accessTokenNode.MoveNext()) || (accessTokenNode.Current == null)) return null;

            result.AccessToken = accessTokenNode.Current.Value;

            // Get the local return URL
            XPathNodeIterator returnUrlNode = responseMessage.Select("/response/return_url");
            if ((!returnUrlNode.MoveNext()) || (returnUrlNode.Current == null)) return null;

            string localReturnUrl = returnUrlNode.Current.Value;

            // Build the return URL
            List<string> returnUrlParameters = new List<string>();

            returnUrlParameters.Add("login_token=" + result.AccessToken);

            result.ReturnURL = _provider.GetRequestUrl("/api/user/redeem-login-token", returnUrlParameters);

            // Return the object
            return result;
        }

        public bool? Update(int userId, string email = null, string username = null, string password = null, string fullName = null, Timezone? timezone = null, bool siteAdmin = false)
        {
            // Verify required parameters
            if (userId <= 0) return null;

            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            requestUrlParameters.Add("user_id=" + userId.ToString());
            if (!String.IsNullOrEmpty(username)) requestUrlParameters.Add("username=" + HttpUtility.UrlEncode(username));
            if (!String.IsNullOrEmpty(password)) requestUrlParameters.Add("password=" + HttpUtility.UrlEncode(password));
            if (!String.IsNullOrEmpty(fullName)) requestUrlParameters.Add("full_name=" + HttpUtility.UrlEncode(fullName));
            if (timezone != null) requestUrlParameters.Add("timezone=" + HttpUtility.UrlEncode(RequestValues.Get(timezone)));
            if (siteAdmin) requestUrlParameters.Add("site_admin=1");

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/user/update", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // Get the User id
            XPathNodeIterator response = responseMessage.Select("/response");
            if ((response.MoveNext()) && (response.Current != null))
            {
                string status = response.Current.GetAttribute("status", "");
                if (status.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    string key = response.Current.GetAttribute("code", "");
                    string message = response.Current.GetAttribute("message", "");
                    switch (key)
                    {
                        case "permission_denied":
                            throw new PermissionDenied(message);

                        case "no_such_user":
                        case "invalid_username":
                            throw new MalformedRequest(message);

                        default:
                            throw new FailedRequest(message);
                    }
                }
            }

            // If nothing pops up, we'll return null
            return null;
        }
    }
}