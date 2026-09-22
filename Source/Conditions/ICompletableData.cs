// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.Conditions
{
    /// <summary>
    /// Data for an entity that can be completed.
    /// </summary>
    public interface ICompletableData : IData
    {
        /// <summary>
        /// True if this data's owning entity is completed.
        /// </summary>
        bool IsCompleted { get; set; }
    }
}