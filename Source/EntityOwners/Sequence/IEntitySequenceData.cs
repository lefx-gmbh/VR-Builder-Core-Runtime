// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.EntityOwners
{
    /// <summary>
    /// Data describing an entity sequence whose current entity is tracked.
    /// </summary>
    public interface IEntitySequenceData<TEntity> : IEntityCollectionData<TEntity>, IEntitySequenceData where TEntity : IEntity
    {
        /// <summary>
        /// Current entity in the sequence.
        /// </summary>
        new TEntity Current { get; set; }
    }

    /// <summary>
    /// A non-generic view over an entity sequence's current entity.
    /// </summary>
    public interface IEntitySequenceData : IData
    {
        /// <summary>
        /// Current entity in the sequence.
        /// </summary>
        IEntity Current { get; }
    }
}