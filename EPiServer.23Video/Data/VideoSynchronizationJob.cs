/*This file is part of 23 Video content provider for EPiServer.
23 Video content provider for EPiServer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
23 Video content provider for EPiServer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
You should have received a copy of the GNU Lesser General Public License along with 23 Video content provider for EPiServer. If not, see http://www.gnu.org/licenses/. */

using System;
using System.Diagnostics;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using System.Collections.Generic;
using EPiServer.Core;

namespace EPiCode.TwentyThreeVideo.Data
{
    [ScheduledPlugIn(DisplayName = "23 Video integration job", DefaultEnabled = true, IntervalType = ScheduledIntervalType.Minutes, IntervalLength = 30)]
    public class VideoSynchronizationJob : ScheduledJobBase
    {
        private bool _stopSignaled;

        public VideoSynchronizationJob()
        {
            IsStoppable = true;
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            _stopSignaled = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            var sw = new Stopwatch();
            sw.Start();
            OnStatusChanged("Starting execution of 23 video synchronization");
            var intermediateVideoDataRepository = new IntermediateVideoDataRepository();
            var firstTry = intermediateVideoDataRepository.TryLoadFromService(out List<BasicContent> videoContent);
            if (!firstTry)
            {
                OnStatusChanged("Loading videos from 23 video failed. Retrying.");
                videoContent = intermediateVideoDataRepository.LoadFromService();
            }
            OnStatusChanged(String.Format("{0} videos and channels found. Now storing to DDS", videoContent.Count));
            intermediateVideoDataRepository.Save(videoContent);
            VideoSynchronizationEventHandler.DataStoreUpdated();

            if (_stopSignaled)
            {
                return "Stop of job was called";
            }
            sw.Stop();
            return "Done" + (firstTry ? string.Empty : " with retry") + ". Time taken " + sw.Elapsed.ToString("g");
        }
    }    
}
