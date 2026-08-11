// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using System.Collections.Generic;

namespace VRBuilder.Core.Attributes
{
    /// <summary>
    /// Marks an <see cref="IList{T}"/> member whose items carry their own metadata attributes and can be
    /// reordered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReorderableListOfAttribute : ListOfAttribute
    {
        /// <summary>
        /// Creates a <see cref="ReorderableListOfAttribute"/> that wraps each reorderable list item with the
        /// given child attributes.
        /// </summary>
        /// <param name="childAttributes">The metadata attributes applied to each list item.</param>
        public ReorderableListOfAttribute(params Type[] childAttributes) : base(childAttributes)
        {
        }
    }
}