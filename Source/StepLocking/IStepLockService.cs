// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.Registry;
using VRBuilder.Core.RestrictiveEnvironment;

namespace VRBuilder.Core.StepLocking
{
    /// <summary>
    /// Allows to implement strategies which restrict interaction with scene objects for specific steps.
    /// </summary>
    public interface IStepLockService : IService<IStepLockConfiguration>
    {
        /// <summary>
        /// Unlocks the restrictive environment and allows interaction with scene objects required for this Step to complete.
        /// </summary>
        /// <param name="data">IStepData of the current step</param>
        /// <param name="manualUnlocked">All LockableProperties which are should be unlocked in addition</param>
        public void Unlock(IStepData data, IEnumerable<LockablePropertyData> manualUnlocked);

        /// <summary>
        /// Locks all unlocked LockableProperties for the current step.
        /// </summary>
        /// <param name="data">IStepData of the current step</param>
        /// <param name="manualUnlocked">All LockableProperties which were unlocked in addition</param>
        public void Lock(IStepData data, IEnumerable<LockablePropertyData> manualUnlocked);

        /// <summary>
        /// Will be called whenever the mode is changed, allows to adapt to lock handling to it.
        /// </summary>
        public void Configure(IMode mode);

        /// <summary>
        /// Will be called once when a process is started.
        /// </summary>
        public void OnProcessStarted(IProcess process);

        /// <summary>
        /// Will be called once when the currently running process finishes.
        /// </summary>
        public void OnProcessFinished(IProcess process);
    }
}
