// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// A behavior performs an action while its step is active (e.g. moving or enabling objects,
    /// playing audio, spawning confetti). Behaviors own <see cref="IBehaviorData"/> and expose their
    /// runtime logic as <see cref="IStageProcess"/>es through the inherited <see cref="IEntity"/> contract.
    /// </summary>
    public interface IBehavior : IEntity, IDataOwner<IBehaviorData>
    {
    }
}