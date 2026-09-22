// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Registry;
using VRBuilder.Core.Settings;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Tracks the <see cref="ISceneObject"/>s of the current scene and their named groups,
    /// providing lookup by guid and by the <see cref="ISceneObjectProperty"/>s attached to them.
    /// </summary>
    public interface ISceneObjectRegistry : IService<ISceneObjectRegistryConfiguration>
    {
        /// <summary>
        /// The named groups of scene objects maintained by the registry.
        /// </summary>
        ISceneObjectGroups SceneObjectGroups { get; }

        /// <summary>
        /// Raised when registered scene objects or their group membership changes.
        /// </summary>
        event Action Changed;

        /// <summary>
        /// Returns if the Guid is registered in the registry.
        /// </summary>
        bool ContainsGuid(Guid guid);

        /// <summary>
        /// Returns all registered scene objects which have the provided guid assigned to them.
        /// </summary>
        IEnumerable<ISceneObject> GetObjects(Guid guid);

        /// <summary>
        /// Returns all registered scene objects with the provided guid and at least one valid property of the specified type.
        /// </summary>
        IEnumerable<T> GetProperties<T>(Guid guid) where T : ISceneObjectProperty;

        /// <summary>
        /// Returns all registered properties of the specified type across all registered scene objects.
        /// </summary>
        IEnumerable<T> GetAllProperties<T>() where T : ISceneObjectProperty;

        /// <summary>
        /// Registers an SceneObject in the registry. If there is an SceneObject with the same name
        /// already registered, an NameNotUniqueException will be thrown. Also if the Guid
        /// is already known an SceneObjectAlreadyRegisteredException will be thrown.
        /// </summary>
        void Register(ISceneObject obj);

        /// <summary>
        /// Removes the SceneObject completely from the Registry.
        /// </summary>
        bool Unregister(ISceneObject obj);

        /// <summary>
        /// Registers all SceneObject in scene, independent of their state.
        /// </summary>
        void RegisterAll();

        /// <summary>
        /// Updates the registry by removing all <see cref="ISceneObject"/> which are not in the scene anymore and adding new ones.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Marks the given scene object as changed so the registry re-evaluates it on the next refresh.
        /// </summary>
        /// <param name="sceneObject">The scene object whose state changed.</param>
        void MarkSceneObjectDirty(ISceneObject sceneObject);
    }
}