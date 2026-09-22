// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Primitives;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Property to Manipulate Movement of a <see cref="ISceneObject"/>.
    /// </summary>
    public interface IMoveProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Disables the physics of the <see cref="ISceneObject"/> especially kinematic.
        /// </summary>
        void DisablePhysics();

        /// <summary>
        /// Moves the <see cref="ISceneObject"/> gradually to final position as lerp based on progress. If you need Easing prepend an Animationcurve like the Unity and Godot Implementations do. 
        /// </summary>
        /// <param name="finalPositionValue">Node/GameObject containing the final position.</param>
        /// <param name="progress">value between 0 and 1 where 0 is closest to start position and 1 closest to the end position.</param>
        /// <param name="animationCurve">optional if null it will interpret progress linear as is.</param>
        void MoveTo(ISceneObject finalPositionValue, float progress, IAnimationCurve animationCurve = null);

        /// <summary>
        /// Enables the physics of the <see cref="ISceneObject"/> especially kinematic.
        /// </summary>
        void EnablePhysics();
    }
}