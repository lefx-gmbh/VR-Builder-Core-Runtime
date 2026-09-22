// copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Interface that allows a Property to detect when a <see cref="ISceneObject"/> enters or exits a trigger collider.
    /// </summary>
    public interface IColliderWithTriggerProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Emitted when a collider enters this trigger.
        /// </summary>
        event Action<IColliderWithTriggerEventArgs> EnteredTriggerAction;

        /// <summary>
        /// Emitted when a collider exits this trigger.
        /// </summary>
        event Action<IColliderWithTriggerEventArgs> ExitedTriggerAction;

        /// <summary>
        /// Returns true if the given <see cref="ISceneObject"/>'s transform is inside this trigger.
        /// </summary>
        /// <param name="sceneObject">The <see cref="ISceneObject"/> to check.</param>
        bool IsTransformInsideTrigger(ISceneObject sceneObject);

        /// <summary>
        /// Teleports the given <see cref="ISceneObject"/> to this trigger's position and fires the entered event.
        /// </summary>
        /// <param name="objs">The <see cref="ISceneObject"/> to teleport.</param>
        void FastForwardEnter(ISceneObject objs);
    }

    /// <summary>
    /// Event arguments for trigger enter and exit events.
    /// </summary>
    public interface IColliderWithTriggerEventArgs
    {
    }
}