// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Registry
{
    /// <summary>
    /// Every Service registered in the <see cref="VRBuilder.Core.Runtime.Registry.ServiceRegistry"/> needs this Interface to be implemented.
    /// It will get it's Configuration to be set, and then it will be initialized.
    /// </summary>
    public interface IService<in T> where T : IServiceConfiguration
    {
        /// <summary>
        /// Configuration callback. The services needs to cast it to it's own Configuration Interface.
        /// </summary>
        /// <param name="configuration"></param>
        void SetConfiguration(T configuration);
    }
}