// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Configuration.Modes
{
    /// <summary>
    /// A process mode that is defined by its name, IConfigurables activation policy and a collection of parameters.
    /// Immutable.
    /// </summary>
    public sealed class ModeService : IModeService
    {
        public IMode ActiveOrDefaultMode { get; set; } = new Mode("Default", new WhitelistTypeRule<IOptional>());

        public IModeHandler ModeHandler { get; set; }
    }
}