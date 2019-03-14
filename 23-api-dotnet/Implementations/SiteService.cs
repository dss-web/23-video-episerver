using System.Collections.Generic;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;

namespace Visual
{
    public class SiteService : ISiteService
    {
        private IApiProvider _provider;

        public SiteService(IApiProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Method for getting a Site object containing information about the site
        /// </summary>
        /// <returns></returns>
        public Domain.Site Get()
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/site/get", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // Get the site id
            XPathNavigator siteIdToken = responseMessage.SelectSingleNode("/response/site_id");
            if (siteIdToken == null) return null;

            // Get the site name
            XPathNavigator siteNameToken = responseMessage.SelectSingleNode("/response/site_name");
            if (siteNameToken == null) return null;

            // Get the product key
            XPathNavigator productKeyToken = responseMessage.SelectSingleNode("/response/product_key");
            if (productKeyToken == null) return null;

            // Get the allow signup variable
            XPathNavigator allowSignupToken = responseMessage.SelectSingleNode("/response/allow_signup_p");
            if (allowSignupToken == null) return null;

            // Get the site key
            XPathNavigator siteKeyToken = responseMessage.SelectSingleNode("/response/site_key");
            if (siteKeyToken == null) return null;

            // Get the license id
            XPathNavigator licenseIdToken = responseMessage.SelectSingleNode("/response/license_id");
            if (licenseIdToken == null) return null;

            // Get the domain
            XPathNavigator domainToken = responseMessage.SelectSingleNode("/response/domain");
            if (domainToken == null) return null;

            // Create result
            try
            {
                Domain.Site result = new Domain.Site
                {
                    SiteId = Helpers.ConvertStringToInteger(siteIdToken.Value),
                    SiteName = siteNameToken.Value,
                    SiteKey = siteKeyToken.Value,
                    ProductKey = productKeyToken.Value,
                    AllowSignup = (allowSignupToken.Value != "f"),
                    LicenseId = Helpers.ConvertStringToInteger(licenseIdToken.Value),
                    Domain = domainToken.Value
                };

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
