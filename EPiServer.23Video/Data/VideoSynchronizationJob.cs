﻿using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Security;

namespace EPiCode.TwentyThreeVideo.Data
{
    [ScheduledPlugIn(DisplayName = "23 Video integration job", DefaultEnabled = true, IntervalType = ScheduledIntervalType.Minutes, IntervalLength = 30)]
    public class VideoSynchronizationJob : JobBase
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
            OnStatusChanged(String.Format("Starting execution of 23 video synchronization"));
            var intermediateVideoDataRepository = new IntermediateVideoDataRepository();
            var videoContent = intermediateVideoDataRepository.LoadFromService();//intermediateVideoDataRepository.LoadFromService();
            OnStatusChanged(String.Format("{0} videos and channels found. Now storing to DDS",videoContent.Count));
            intermediateVideoDataRepository.Save(videoContent);
            VideoSynchronizationEventHandler.DataStoreUpdated();

           // VideoSynchronizationEventHandler.DataStoreUpdated();
            if (_stopSignaled)
            {
                return "Stop of job was called";
            }
            sw.Stop();
            return "Done. Time taken " + sw.Elapsed.ToString("g");
        }
    }

    
}