// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0


using System;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Defines at which execution stages of a behavior the behavior should run.
    /// </summary>
    [Flags]
    public enum BehaviorExecutionStages
    {
        /// <summary>
        /// Run the behavior during the activation of the behavior.
        /// </summary>
        Activation = 1 << 0,

        /// <summary>
        /// Run the behavior during the deactivation of the behavior.
        /// </summary>
        Deactivation = 1 << 1,

        /// <summary>
        /// Run the behavior during both activation and deactivation.
        /// </summary>
        ActivationAndDeactivation = ~0,

        /// <summary>
        /// Do not run the behavior at any stage.
        /// </summary>
        None = 0,
    }
}