// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

﻿using System;
using System.Collections.ObjectModel;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Event arguments raised when the behavior collection of an <see cref="IBehaviorParent"/> changes.
    /// </summary>
    public class BehaviorCollectionChangedEventArgs : EventArgs
    {
    }

    /// <summary>
    /// Contract for objects that own a collection of behaviors.
    /// </summary>
    public interface IBehaviorParent
    {
        /// <summary>
        /// List of behaviors associated with this object.
        /// </summary>
        ReadOnlyCollection<IBehavior> Behaviors { get; }

        /// <summary>
        /// Invoked when behavior is added or removed from this object.
        /// </summary>
        event EventHandler<BehaviorCollectionChangedEventArgs> BehaviorCollectionChanged;

        /// <summary>
        /// Returns true if this object has given behavior.
        /// </summary>
        bool CheckHasBehavior(IBehavior behavior);

        /// <summary>
        /// Add behavior to this object. Implementation of this method should invoke BehaviorCollectionChanged event.
        /// </summary>
        /// <param name="behavior">Behavior to be added.</param>
        void AddBehavior(IBehavior behavior);

        /// <summary>
        /// Insert the <paramref name="behavior"/> into the collection of behaviors at <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index at which the behavior is inserted.</param>
        /// <param name="behavior">Behavior to be inserted.</param>
        void InsertBehavior(int index, IBehavior behavior);

        /// <summary>
        /// Remove behavior from this object. Implementation of this method should invoke BehaviorCollectionChanged event.
        /// </summary>
        bool RemoveBehavior(IBehavior behavior);
    }
}