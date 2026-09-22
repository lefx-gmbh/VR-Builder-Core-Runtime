// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using VRBuilder.Core.Properties;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Arguments for UniqueIdChanged event.
    /// </summary>
    public class UniqueIdChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new unique id.
        /// </summary>
        public readonly Guid NewId;

        /// <summary>
        /// The previous unique id.
        /// </summary>
        public readonly Guid PreviousId;

        /// <summary>
        /// Creates event args for a unique id change.
        /// </summary>
        /// <param name="previousId">The previous unique id.</param>
        /// <param name="newId">The new unique id.</param>
        public UniqueIdChangedEventArgs(Guid previousId, Guid newId)
        {
            NewId = newId;
            PreviousId = previousId;
        }
    }

    /// <summary>
    /// Gives the possibility to easily identify targets for Conditions, Behaviors and so on.
    /// </summary>
    public interface ISceneObject : ILockable, IGuidContainer
    {
        /// <summary>
        /// Unique Guid for each entity, which is required
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Properties on the scene object.
        /// </summary>
        ICollection<ISceneObjectProperty> Properties { get; }

        /// <summary>
        /// Called when the object's object id has been changed.
        /// </summary>
        event EventHandler<UniqueIdChangedEventArgs> ObjectIdChanged;

        /// <summary>
        /// True if the scene object has a property of the specified type.
        /// </summary>
        bool CheckHasProperty<T>() where T : ISceneObjectProperty;

        /// <summary>
        /// True if the scene object has a property of the specified type.
        /// </summary>
        bool CheckHasProperty(Type type);

        /// <summary>
        /// Validates properties on the scene object.
        /// </summary>        
        void ValidateProperties(IEnumerable<Type> properties);

        /// <summary>
        /// Returns a property of the specified type.
        /// </summary>
        T GetProperty<T>() where T : ISceneObjectProperty;

        /// <summary>
        /// Gives the object a new specified unique ID.
        /// </summary>
        void SetObjectId(Guid guid);
    }
}