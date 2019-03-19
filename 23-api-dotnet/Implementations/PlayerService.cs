using System.Collections.Generic;
using System.Xml.XPath;
using DotNetOpenAuth.Messaging;

namespace Visual
{
    public class PlayerService : IPlayerService
    {
        private IApiProvider _provider;

        public PlayerService(IApiProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Get a list of players defined by the default request parameters
        /// </summary>
        /// <returns></returns>
        public List<Domain.Player> GetList()
        {
            // Build request URL
            List<string> requestUrlParameters = new List<string>();

            // Do the request
            MessageReceivingEndpoint requestMessage = new MessageReceivingEndpoint(_provider.GetRequestUrl("/api/player/list", requestUrlParameters), HttpDeliveryMethods.GetRequest);

            XPathNavigator responseMessage = _provider.DoRequest(requestMessage);
            if (responseMessage == null) return null;

            // List all the videos
            XPathNodeIterator players = responseMessage.Select("/response/player");
            List<Domain.Player> result = new List<Domain.Player>();

            while (players.MoveNext())
            {
                if (players.Current == null) return null;

                // Create the domain Tag
                Domain.Player playerModel = new Domain.Player
                                          {
                                              Default = (players.Current.GetAttribute("default_p", "") == "1"),
                                              Name = players.Current.GetAttribute("player_name", ""),
                                              Id = players.Current.GetAttribute("player_id", "")
                                          };

                result.Add(playerModel);
            }

            return result;
        }
    }
}
