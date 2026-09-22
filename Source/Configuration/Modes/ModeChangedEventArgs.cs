// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;

namespace VRBuilder.Core.Configuration.Modes
{
    /// <summary>
    /// This is a <see cref="EventArgs"/> used for <see cref="IMode"/> changes.
    /// If you want so see more about EventArgs, please visit: https://docs.microsoft.com/en-us/dotnet/standard/events/
    /// </summary>
    public class ModeChangedEventArgs : EventArgs
    {
        public ModeChangedEventArgs(IMode mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// The newly activated <see cref="IMode"/>.
        /// </summary>
        public IMode Mode { get; private set; }
    }
}