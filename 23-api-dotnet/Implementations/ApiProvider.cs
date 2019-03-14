using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using System.Net;
using Visual.Exceptions;
using System.IO;
using Visual.Utilities;

namespace Visual
{
    public class ApiProvider : IApiProvider
    {
        // * Consumer objects
        private WebConsumer _oAuthConsumer;
        private ServiceProviderDescription _oAuthProviderDescription = new ServiceProviderDescription();
        private InMemoryTokenManager _oAuthTokenManager;
        private WebProxy _proxy;

        // * Variables
        private string _consumerDomain;
        private string _consumerKey;
        private string _consumerSecret;

        private string _accessToken;
        private string _accessTokenSecret;

        private bool _httpSecure;

        // * Constructor
		/// <summary>
		/// Creates a 23 API service repository, that relies only on public access to the API
		/// </summary>
		/// <param name="consumerDomain">
		/// Domain name
		/// </param>
		public ApiProvider(string consumerDomain)
		{
			_consumerDomain = consumerDomain;
		}
		
        /// <summary>
        /// Creates a 23 API service repository, that requires further authentication approval.
        /// When using this constructor, you should consider rewriting the token manager (InMemoryTokenManager) to let your application handle access tokens
        /// </summary>
        /// <param name="consumerDomain">Domain name</param>
        /// <param name="consumerKey">Consumer key</param>
        /// <param name="consumerSecret">Consumer secret</param>
        public ApiProvider(string consumerDomain, string consumerKey, string consumerSecret)
            : this(consumerDomain, consumerKey, consumerSecret, null, null, false)
        {

        }

        /// <summary>
        /// Creates a 23 API service repository, that doesn't require further authentication. Account must be "privileged"
        /// </summary>
        /// <param name="consumerDomain">Domain name</param>
        /// <param name="consumerKey">Consumer key</param>
        /// <param name="consumerSecret">Consumer secret</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="accessTokenSecret">Access token secret</param>
        public ApiProvider(string consumerDomain, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, bool httpSecure)
        {
            // Save the authentication keys
            _consumerDomain = consumerDomain;

            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;

            _httpSecure = httpSecure;

            string protocol = httpSecure ? "https://" : "http://";

            // Adjust timeout for requests to the domain to allow for large file uploads
            WebRequest.RegisterPrefix(protocol + _consumerDomain, TwentyThreeCreatorRequestCreator.TwentyThreeHttp);

            // Open the OAuth consumer connection
            _oAuthProviderDescription.AccessTokenEndpoint = new MessageReceivingEndpoint(protocol + "api.visualplatform.net/oauth/access_token", HttpDeliveryMethods.GetRequest);
            _oAuthProviderDescription.RequestTokenEndpoint = new MessageReceivingEndpoint(protocol + "api.visualplatform.net/oauth/request_token", HttpDeliveryMethods.GetRequest);
            _oAuthProviderDescription.ProtocolVersion = ProtocolVersion.V10a;
            _oAuthProviderDescription.UserAuthorizationEndpoint = new MessageReceivingEndpoint(protocol + "api.visualplatform.net/oauth/authorize", HttpDeliveryMethods.GetRequest);
            _oAuthProviderDescription.TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() };

            _oAuthTokenManager = new InMemoryTokenManager(_consumerKey, _consumerSecret);

            _oAuthConsumer = new WebConsumer(_oAuthProviderDescription, _oAuthTokenManager);

            if (accessToken != null)
            {
                _accessToken = accessToken;
                _accessTokenSecret = accessTokenSecret;

                _oAuthTokenManager.ExpireRequestTokenAndStoreNewAccessToken(_consumerKey, "", _accessToken, _accessTokenSecret);
            }

            _oAuthConsumer.Channel.AssertBoundary();
        }

        // ***** Internal functions *****
        public XPathNavigator DoRequest(MessageReceivingEndpoint message)
        {
            return DoRequest(message, null);
        }

        public XPathNavigator DoRequest(MessageReceivingEndpoint message, List<MultipartPostPart> parameters)
        {
			// Initialize the response XML document
			XDocument responseDocument = null;
			
			// Perform the request based on the authentication
			if (_oAuthConsumer == null)
			{
				// Multipart requests are only available to authenticated API accesses at the moment
				if (parameters != null)
					throw new NotImplementedException();
				
				// Construct the request
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(message.Location);
				
				if ((message.AllowedMethods & HttpDeliveryMethods.PostRequest) == HttpDeliveryMethods.PostRequest)
					request.Method = "POST";

				if (_proxy != null)
	                request.Proxy = _proxy;
				
				// Get and parse the response
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new MalformedRequest(new StreamReader(response.GetResponseStream()).ReadToEnd());
                }
				
				responseDocument = XDocument.Load(XmlReader.Create(response.GetResponseStream(), new XmlReaderSettings()));
			}
			else
			{
	            // Verify authentication
	            if (_accessToken == null)
	            {
	                AuthorizedTokenResponse accessTokenResponse = _oAuthConsumer.ProcessUserAuthorization();
	                if (accessTokenResponse != null) _accessToken = accessTokenResponse.AccessToken;
	                else if (_accessToken == null) _oAuthConsumer.Channel.Send(_oAuthConsumer.PrepareRequestUserAuthorization());
	            }
	
				// Construct the request
	            HttpWebRequest request = (parameters == null ? _oAuthConsumer.PrepareAuthorizedRequest(message, _accessToken) : _oAuthConsumer.PrepareAuthorizedRequest(message, _accessToken, parameters));
	            if (_proxy != null)
	                request.Proxy = _proxy;

                IncomingWebResponse response = _oAuthConsumer.Channel.WebRequestHandler.GetResponse(request);

                if (response.Status != HttpStatusCode.OK)
                {
                    throw new MalformedRequest(new StreamReader(response.ResponseStream).ReadToEnd());
                }
	
				// Parse the response
	            responseDocument = XDocument.Load(XmlReader.Create(response.GetResponseReader()));
			}

            // Establish navigator and validate response
            XPathNavigator responseNavigation = responseDocument.CreateNavigator();

            XPathNodeIterator responseCheckIterator = responseNavigation.Select("/response");
            if (responseCheckIterator.Count == 0)
            {
                throw new MalformedRequest(responseDocument.ToString());
            }
            while (responseCheckIterator.MoveNext())
            {
                if (responseCheckIterator.Current == null) return null;

                if (responseCheckIterator.Current.GetAttribute("status", "") != "ok")
                {
                    string code = responseCheckIterator.Current.GetAttribute("code", "");
                    string msg = responseCheckIterator.Current.GetAttribute("message", "");
                    switch (code)
                    {
                        default:
                            throw new MalformedRequest(msg);

                        case "invalid_oauth_site":
                        case "invalid_oauth_user":
                        case "invalid_signature":
                        case "token_required":
                            throw new InvalidCredentials(msg);

                        case "permission_denied":
                            throw new PermissionDenied(msg);
                    }
                }
            }

            // All should be good, return the document
            return responseNavigation;
        }

        public string GetRequestUrl(string method, List<string> parameters)
        {
            string protocol = _httpSecure ? "https://" : "http://";
            return protocol + _consumerDomain + method + (parameters != null ? (parameters.Count > 0 ? "?" + String.Join("&", parameters.ToArray()) : "") : "");
        }

        public void SetProxy(string uri, string username = null, string password = null, string domain = null)
        {
            _proxy = new WebProxy(new Uri(uri));
            if ((!string.IsNullOrEmpty(username)) || (!string.IsNullOrEmpty(password)))
                _proxy.Credentials = !string.IsNullOrEmpty(domain) ? new NetworkCredential(username, password, domain) : new NetworkCredential(username, password);
        }
    }
}