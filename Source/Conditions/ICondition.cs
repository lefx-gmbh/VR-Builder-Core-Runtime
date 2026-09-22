// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.Conditions
{
    /// <summary>
    /// A condition is a completable process entity used by <see cref="ITransition"/>s: it completes when
    /// its criterion is met (e.g. an object is in range or a value comparison returns true), which lets
    /// the transition advance to the next step. Conditions own <see cref="IConditionData"/> and can be cloned.
    /// </summary>
    public interface ICondition : ICompletableEntity, IDataOwner<IConditionData>
    {
    }
}