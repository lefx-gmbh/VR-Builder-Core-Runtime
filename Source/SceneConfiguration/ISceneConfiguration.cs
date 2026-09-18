// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using VRBuilder.Core.Registry;

namespace VRBuilder.Core.Configuration
{
    /// <summary>
    /// Marker interface for the configuration of scene-related services.
    /// </summary>
    public interface ISceneConfiguration : IServiceConfiguration
    {
        bool IsPropertyAllowed(Type propertyType);
        void SetPropertyAllowed(Type propertyType, bool allowed);
    }
}