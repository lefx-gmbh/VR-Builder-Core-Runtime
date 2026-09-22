// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Interface that allows a Property to enable or disable components on a <see cref="ISceneObject"/>.
    /// </summary>
    public interface IModifySceneComponentProperty : ISceneObjectProperty
    {
        //TODO: could also be GUID instead of type string
        /// <summary>
        /// Finds all components matching <paramref name="componentType"/> on the <see cref="ISceneObject"/> and sets their enabled state.
        /// </summary>
        /// <param name="componentType">The type name of the component to enable or disable.</param>
        /// <param name="setEnabled">True to enable, false to disable the matching components.</param>
        void SetComponentActive(string componentType, bool setEnabled);
    }
}