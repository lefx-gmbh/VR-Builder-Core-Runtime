// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Registry
{
    /// <summary>
    /// Base contract for all service configurations. Used as the generic constraint by
    /// <see cref="IService{T}"/> to tie a service to the configuration it operates on.
    /// </summary>
    public interface IServiceConfiguration
    {
    }
}