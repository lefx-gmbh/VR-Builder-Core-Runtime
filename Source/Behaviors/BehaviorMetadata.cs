// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System.Runtime.Serialization;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Metadata for behaviors, storing editor UI state.
    /// </summary>
    [DataContract(IsReference = true)]
    public class BehaviorMetadata : IMetadata
    {
        /// <summary>
        /// If <c>true</c>, the behavior is expanded in the process inspector.
        /// </summary>
        [DataMember]
        public bool IsFoldedOut { get; set; }
    }
}