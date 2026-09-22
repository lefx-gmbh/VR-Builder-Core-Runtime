// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core
{
    /// <summary>
    /// A process is the top-level container of a training: it owns an ordered collection of
    /// <see cref="IChapter"/>s and, through its <see cref="ILifeCycle"/>, drives execution from
    /// chapter to chapter. The running instance is driven by the <see cref="IProcessRunner"/> service.
    /// </summary>
    public interface IProcess : IEntity, IDataOwner<IProcessData>
    {
        /// <summary>
        /// Utility data used by editor.
        /// </summary>
        ProcessMetadata ProcessMetadata { get; }
    }
}