// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using System.Reflection;
using VRBuilder.Core.Behaviors;

namespace VRBuilder.Core.Attributes
{
    /// <summary>
    /// Adds an "Is Blocking" toggle to the element's body. The toggle only appears when the element is a
    /// behavior that implements <see cref="IBackgroundBehaviorData"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DrawIsBlockingToggleAttribute : MetadataAttribute
    {
        /// <inheritdoc />
        public override object GetDefaultMetadata(MemberInfo owner)
        {
            return null;
        }

        /// <inheritdoc />
        public override bool IsMetadataValid(object metadata)
        {
            return metadata == null;
        }
    }
}