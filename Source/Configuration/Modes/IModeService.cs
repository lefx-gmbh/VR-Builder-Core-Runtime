// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Configuration.Modes
{
    /// <summary>
    /// The interface of a process mode. A process mode determines if an entity has to be skipped and provides configurable entities with parameters.
    /// </summary>
    public interface IModeService
    {
        /// <summary>
        /// Get or set the default or active mode.
        /// </summary>
        IMode ActiveOrDefaultMode { get; set; }
        
        /// <summary>
        /// Get or set the mode handler.
        /// </summary>
        IModeHandler ModeHandler { get; set; }
    }
}