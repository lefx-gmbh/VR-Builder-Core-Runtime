// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Utils
{
    /// <summary>
    /// Utility methods for obtaining unique identifiers from scene objects and their properties.
    /// </summary>
    public static class ProcessReferenceUtils
    {
        /// <summary>
        /// Returns the unique identifier of the scene object that owns the given <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The property whose owning scene object identifier is returned.</param>
        /// <returns>The <see cref="Guid"/> of the owning scene object, or <see cref="Guid.Empty"/> when <paramref name="property"/> is null.</returns>
        public static Guid GetUniqueIdFrom(ISceneObjectProperty property)
        {
            if (property == null)
            {
                return Guid.Empty;
            }

            return GetUniqueIdFrom(property.SceneObject);
        }

        /// <summary>
        /// Returns the unique identifier of the given <paramref name="sceneObject"/>.
        /// </summary>
        /// <param name="sceneObject">The scene object whose identifier is returned.</param>
        /// <returns>The <see cref="Guid"/> of the scene object, or <see cref="Guid.Empty"/> when <paramref name="sceneObject"/> is null.</returns>
        public static Guid GetUniqueIdFrom(ISceneObject sceneObject)
        {
            if (sceneObject == null)
            {
                return Guid.Empty;
            }

            return sceneObject.Guid;
        }
    }
}