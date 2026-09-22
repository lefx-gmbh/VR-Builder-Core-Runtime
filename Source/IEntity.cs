// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using VRBuilder.Core.Configuration.Modes;

namespace VRBuilder.Core
{
    /// <summary>
    /// Base contract for every component of a process (chapters, steps, behaviors, conditions, transitions).
    /// An entity owns an <see cref="ILifeCycle"/> that drives it through the <see cref="Stage"/>s; for each
    /// stage it provides the matching <see cref="IStageProcess"/> via the Get*Process methods, and
    /// <see cref="Configure"/> wires the entity to the active process mode.
    /// Do not implement this interface directly — derive from <c>Behavior</c>, <c>Condition</c>,
    /// <c>Step</c>, <c>Chapter</c> or <c>Process</c> instead.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Unique identifier of the entity.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Generates a new unique identifier for the entity.
        /// </summary>
        void RegenerateId();

        /// <summary>
        /// The entity's life cycle.
        /// </summary>
        ILifeCycle LifeCycle { get; }

        /// <summary>
        /// Entity parent to this entity.
        /// </summary>
        IEntity Parent { get; set; }

        /// <summary>
        /// Returns a new instance of a process for the Activating <seealso cref="Stage"/>.
        /// </summary>
        IStageProcess GetActivatingProcess();

        /// <summary>
        /// Returns a new instance of a process for the Active <seealso cref="Stage"/>.
        /// </summary>
        IStageProcess GetActiveProcess();

        /// <summary>
        /// Returns a new instance of a process for the Deactivating <seealso cref="Stage"/>.
        /// </summary>
        IStageProcess GetDeactivatingProcess();

        /// <summary>
        /// Returns a new instance of a process for the Aborting <seealso cref="Stage"/>.
        /// </summary>
        IStageProcess GetAbortingProcess();

        /// <summary>
        /// Configures the entity according to the given <paramref name="mode"/>.
        /// </summary>
        void Configure(IMode mode);

        /// <summary>
        /// Called every frame during the Unity's update.
        /// </summary>
        void Update();
    }
}