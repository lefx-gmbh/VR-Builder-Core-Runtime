// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;

namespace VRBuilder.Core
{
    /// <summary>
    /// EventArgs for process events.
    /// </summary>
    public class ProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Active Chapter.
        /// </summary>
        public readonly IChapter Chapter;

        /// <summary>
        /// Active process.
        /// </summary>
        public readonly IProcess Process;

        /// <summary>
        /// Active Step.
        /// </summary>
        public readonly IStep Step;

        /// <summary>
        /// Creates event args for the given process, capturing its current chapter and step.
        /// </summary>
        /// <param name="process">The active process.</param>
        public ProcessEventArgs(IProcess process)
        {
            Process = process;
            Chapter = process.Data.Current;
            if (Chapter != null)
            {
                Step = Chapter.Data.Current;
            }
        }
    }
}