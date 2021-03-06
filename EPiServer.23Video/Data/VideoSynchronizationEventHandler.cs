﻿/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */
using EPiCode.TwentyThreeVideo.Provider;
using EPiServer.Core;
using EPiServer.Events;
using EPiServer.Events.Clients;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System;
using System.Linq;

namespace EPiCode.TwentyThreeVideo.Data
{

    public class VideoSynchronizationEventHandler : EPiServer.PlugIn.PlugInAttribute
    {
        private static readonly ILogger _log = LogManager.GetLogger();
        private static readonly Guid _dataStoreUpdateEventId = new Guid("{26A1CA35-1CBD-44a7-8243-5E80D79F3F26}");
        private static readonly Guid _dataStoreUpdateRaiserId = new Guid("{6180555A-7A0E-4485-B1B1-44BF6E4D4A0D}");

        public static void Start()
        {
            try
            {
                if (Event.EventsEnabled)
                {

                    _log.Debug("Begin: Initializing Data Store Invalidation Handler on '{0}'", Environment.MachineName);

                    _log.Debug("Domain ID: '{0}', Friendly Name: '{1}', Basedir: '{2}', Thread: '{3}'",
                        AppDomain.CurrentDomain.Id.ToString(),
                        AppDomain.CurrentDomain.FriendlyName,
                        AppDomain.CurrentDomain.BaseDirectory,
                        System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                    // Listen to events
                    Event dataStoreInvalidationEvent = Event.Get(_dataStoreUpdateEventId);
                    dataStoreInvalidationEvent.Raised += new EventNotificationHandler(dataStoreInvalidationEvent_Raised);

                    _log.Debug("End: Initializing Data Store Invalidation Handler on '{0}'", Environment.MachineName);

                }
                else
                    _log.Debug("NOT Initializing Data Store Invalidation Handler on '{0}'. Events are disabled for this site.", Environment.MachineName);
            }
            catch (Exception ex)
            {
                _log.Error("Cannot Initialize Data Store Invalidation Handler Correctly", ex);
            }
        }

        static void dataStoreInvalidationEvent_Raised(object sender, EventNotificationEventArgs e)
        {
            _log.Debug("dataStoreInvalidationEvent '{2}' handled - raised by '{0}' on '{1}'", e.RaiserId, Environment.MachineName, e.EventId);
            _log.Debug("Begin: refreshing video items on: '{0}'", Environment.MachineName);
            var provider =
                ServiceLocator.Current.GetInstance<IContentProviderManager>().GetProvider(Constants.ProviderKey) as
                TwentyThreeVideoProvider;
            var intermediateVideoDataRepository = new IntermediateVideoDataRepository();
            var videoItems = intermediateVideoDataRepository.Load();
            provider.RefreshItems(videoItems.ToList());
            _log.Debug("End: refreshing video items on '{0}'", Environment.MachineName);
            // CustomRedirectHandler handler = CustomRedirectHandler.Current;

        }
        public static void DataStoreUpdated()
        {
            // Video content is changing, notify the other servers
            Event dataStoreInvalidateEvent = EPiServer.Events.Clients.Event.Get(_dataStoreUpdateEventId);
            // Raise event
            dataStoreInvalidateEvent.Raise(_dataStoreUpdateRaiserId, null);

        }
    }
}