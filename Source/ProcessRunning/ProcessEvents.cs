// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;

namespace VRBuilder.Core.ProcessRunning
{
    /// <summary>
    /// Holds the events that are raised during process execution, from initialization through completion,
    /// including chapter and step activation and manual fast forward.
    /// </summary>
    public class ProcessEvents
    {
        /// <summary>
        /// Will be called each time a chapter activates.
        /// </summary>
        public EventHandler<ProcessEventArgs> ChapterStarted;

        /// <summary>
        /// Will be called when manual fast forward is triggered.
        /// </summary>
        public EventHandler<FastForwardProcessEventArgs> FastForwardStep;

        /// <summary>
        /// Will be called when the process finishes.
        /// </summary>
        public EventHandler<ProcessEventArgs> ProcessFinished;

        /// <summary>
        /// Will be called when the process has been initialized.
        /// </summary>
        public EventHandler<ProcessEventArgs> ProcessInitialized;

        /// <summary>
        /// Will be called before the process is setup internally.
        /// </summary>
        public EventHandler<ProcessEventArgs> ProcessSetup;

        /// <summary>
        /// Will be called on process start.
        /// </summary>
        public EventHandler<ProcessEventArgs> ProcessStarted;

        /// <summary>
        /// Will be called each time a step activates.
        /// </summary>
        public EventHandler<ProcessEventArgs> StepStarted;
    }
}