// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
	/// <summary>
	/// Property for lockable objects in the scene.
	/// </summary>
	public interface ILockableProperty: ISceneObjectProperty, ILockable
	{
        /// <summary>
        /// Decides if the property will be locked when the parent scene object is locked.
        /// </summary>
        public bool InheritSceneObjectLockState { get; set; }

        /// <summary>
        /// On default the lockable property will use this value to determine if its locked at the end of a step.
        /// </summary>
        public bool EndStepLocked { get; }
	}
}