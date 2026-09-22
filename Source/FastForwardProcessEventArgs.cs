// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core
{
    /// <summary>
    /// EventArgs for fast forward process events.
    /// </summary>
    public class FastForwardProcessEventArgs : ProcessEventArgs
    {
        /// <summary>
        /// Completed transition
        /// </summary>
        public readonly ITransition CompletedTransition;

        /// <summary>
        /// Creates event args for a fast-forward event with the completed transition.
        /// </summary>
        /// <param name="transition">The completed transition.</param>
        /// <param name="process">The active process.</param>
        public FastForwardProcessEventArgs(ITransition transition, IProcess process) : base(process)
        {
            CompletedTransition = transition;
        }
    }
}