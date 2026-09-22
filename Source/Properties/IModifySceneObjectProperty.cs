// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Interface that allows a Property to enable or disable a <see cref="ISceneObject"/>.
    /// </summary>
    public interface IModifySceneObjectProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Enables or disables the <see cref="ISceneObject"/>.
        /// </summary>
        /// <param name="setEnabled">True to enable, false to disable the scene object.</param>
        void SetActive(bool setEnabled);
    }
}