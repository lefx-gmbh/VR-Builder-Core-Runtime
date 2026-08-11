// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Reflection;

namespace VRBuilder.Core.Attributes
{
    /// <summary>
    /// Marks an <see cref="IList{T}"/> member so that when it renders empty, the editor inserts one default
    /// instance of the declared element type. The list never stays empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeepPopulatedAttribute : MetadataAttribute
    {
        /// <summary>
        /// Defines the type of an element to create.
        /// </summary>
        private readonly Type defaultType;

        /// <summary>
        /// Creates a <see cref="KeepPopulatedAttribute"/> for the given element type.
        /// </summary>
        /// <param name="type">The type of the default element created when the list is empty.</param>
        public KeepPopulatedAttribute(Type type)
        {
            defaultType = type;
        }

        /// <inheritdoc />
        public override object GetDefaultMetadata(MemberInfo owner)
        {
            return defaultType;
        }

        /// <inheritdoc />
        public override bool IsMetadataValid(object metadata)
        {
            return metadata is Type;
        }
    }
}