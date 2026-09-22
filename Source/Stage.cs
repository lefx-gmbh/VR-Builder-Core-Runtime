// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core
{
    /// <summary>
    /// All possible states of an <see cref="IEntity"/>.
    /// </summary>
    public enum Stage
    {
        /// <summary>
        /// The entity is not running.
        /// </summary>
        Inactive,

        /// <summary>
        /// The entity is starting up.
        /// </summary>
        Activating,

        /// <summary>
        /// The entity is running.
        /// </summary>
        Active,

        /// <summary>
        /// The entity is shutting down.
        /// </summary>
        Deactivating,

        /// <summary>
        /// The entity is being aborted.
        /// </summary>
        Aborting
    }
}