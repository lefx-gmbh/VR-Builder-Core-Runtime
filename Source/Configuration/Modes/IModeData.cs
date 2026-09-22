// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Configuration.Modes
{
    /// <summary>
    /// Data that carries the <see cref="IModeService"/> which determines how its owner behaves.
    /// </summary>
    public interface IModeData : IData
    {
        IMode Mode { get; set; }
    }
}