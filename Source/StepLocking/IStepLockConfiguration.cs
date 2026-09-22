// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Registry;

namespace VRBuilder.Core.StepLocking
{
    /// <summary>
    /// Configuration that controls when the step lock service locks the process.
    /// </summary>
    public interface IStepLockConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// Whether the process is locked when it starts.
        /// </summary>
        bool LockOnProcessStart { get; set; } // = true;

        /// <summary>
        /// Whether the process is locked when it finishes.
        /// </summary>
        bool LockOnProcessFinished { get; set; } // = true;
    }
}