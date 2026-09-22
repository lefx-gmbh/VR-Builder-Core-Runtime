// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Registry;

namespace VRBuilder.Core.Localization
{
    /// <summary>
    /// Configuration for the <see cref="ILanguageService"/> service.
    /// Implementations are registered alongside the runner to control its behaviour.
    /// </summary>
    public interface ILanguageConfiguration: IServiceConfiguration
    {
    }
}
