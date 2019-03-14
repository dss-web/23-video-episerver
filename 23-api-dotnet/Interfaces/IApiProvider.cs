using System.Collections.Generic;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;

namespace Visual
{
	/// <summary>
	/// Provider for API access
	/// </summary>
    public interface IApiProvider
    {
		/// <summary>
		/// Perform a request
		/// </summary>
		/// <returns>
		/// An XPathNavigator instance for the result or null upon error
		/// </returns>
		/// <param name="message">
		/// Message endpoint
		/// </param>
        XPathNavigator DoRequest(MessageReceivingEndpoint message);
		
		/// <summary>
		/// Perform a multipart post request. Only available when API credentials are used
		/// </summary>
		/// <returns>
		/// An XPathNavigator instance for the result or null upon error.
		/// </returns>
		/// <param name="message">
		/// Message endpoint
		/// </param>
		/// <param name="parameters">
		/// Multipart post parts
		/// </param>
        XPathNavigator DoRequest(MessageReceivingEndpoint message, List<MultipartPostPart> parameters);
		
		/// <summary>
		/// Gets the request URL
		/// </summary>
		/// <returns>
		/// The request URL
		/// </returns>
		/// <param name="method">
		/// API endpoint
		/// </param>
		/// <param name="parameters">
		/// List of properly encoded parameters
		/// </param>
        string GetRequestUrl(string method, List<string> parameters);
		
		/// <summary>
		/// Sets the proxy for requests
		/// </summary>
		/// <param name="uri">
		/// URI of the proxy
		/// </param>
		/// <param name="username">
		/// Username to use for authentication
		/// </param>
		/// <param name="password">
		/// Password to use for authentication
		/// </param>
		/// <param name="domain">
		/// Domain to use for authentication
		/// </param>
        void SetProxy(string uri, string username = null, string password = null, string domain = null);
    }
}