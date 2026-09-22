// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Registry;

namespace VRBuilder.Core.ProcessRunning
{
    /// <summary>
    /// Configuration for the <see cref="IProcessRunner"/> service.
    /// Implementations are registered alongside the runner to control its behaviour.
    /// </summary>
    public interface IProcessRunnerConfiguration: IServiceConfiguration
    {
        /// <summary>
        /// If <c>true</c>, process events are reset when the current scene is unloaded.
        /// </summary>
        bool ResetEventsOnSceneUnload { get; }
    }
}
