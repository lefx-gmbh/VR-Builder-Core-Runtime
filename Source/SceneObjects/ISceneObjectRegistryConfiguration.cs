// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Helpers;
using VRBuilder.Core.Registry;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Configuration for <see cref="ISceneObjectRegistry"/> defining how scene objects are found,
    /// identified, and persisted when a duplicate guid is detected.
    /// </summary>
    public interface ISceneObjectRegistryConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// Provides a way to discover all scene objects without coupling the core to a specific engine.
        /// </summary>
        ISceneObjectFinder SceneObjectFinder { get; }

        /// <summary>
        /// Provides a per-instance identity for scene objects, distinguishing instances that share
        /// the same serialized guid.
        /// </summary>
        ISceneObjectIdentity SceneObjectIdentity { get; }

        /// <summary>
        /// Handler invoked when a duplicate guid is detected, so the engine-specific layer can
        /// persist the reassigned guid.
        /// </summary>
        IEditorPrefabHandler EditorPrefabHandler { get; }
    }
}