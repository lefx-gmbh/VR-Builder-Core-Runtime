// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Interface for creating and managing confetti effects (maybe general particle effects later) on a scene object.
    /// </summary>
    public interface IEffectProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Emitted when a confetti machine has been created and is ready for activation.
        /// </summary>
        event Action ConfettiMachineCreatedAction;

        /// <summary>
        /// Number of currently tracked confetti machines.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Creates a confetti machine at the <see cref="ISceneObject"/>'s position.
        /// </summary>
        void CreateConfettiMachine();

        /// <summary>
        /// Creates a confetti machine above each tracked user's head, offset by <paramref name="distanceAboveUser"/>.
        /// </summary>
        /// <param name="distanceAboveUser">Vertical offset above the user's head position.</param>
        void CreateConfettiMachineAboveUser(float distanceAboveUser = 0f);

        /// <summary>
        /// Loads the confetti machine prefab from the specified path.
        /// </summary>
        /// <param name="confettiMachinePrefabPath">Resource path to the confetti prefab. without the Engine specific prefix</param>
        /// <returns>True if the prefab was loaded successfully.</returns>
        bool LoadConfettiMachinePrefab(string confettiMachinePrefabPath);

        /// <summary>
        /// Activates the most recently created confetti machine with the specified parameters.
        /// </summary>
        /// <param name="areaRadius">Radius of the confetti effect area.</param>
        /// <param name="duration">Duration of the confetti effect.</param>
        void ActivateConfettiMachine(float areaRadius, float duration);

        /// <summary>
        /// Destroys all spawned confetti machines and clears the internal list.
        /// </summary>
        void ClearConfettiMachines();
    }
}