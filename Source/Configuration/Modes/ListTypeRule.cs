// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace VRBuilder.Core.Configuration.Modes
{
    /// <summary>
    /// List-based <see cref="TypeRule{TValueBase}"/> used by the mode system to declare which concrete
    /// types are enabled in a process mode. The rule keeps an allow-list of types; <see cref="Add{T}"/>
    /// returns a new rule instance with an additional allowed type, so modes can be composed without
    /// mutating shared instances.
    /// </summary>
    public abstract class ListTypeRule<TRecursive, TValueBase> : TypeRule<TValueBase> where TRecursive : ListTypeRule<TRecursive, TValueBase>, new()
    {
        private HashSet<Type> storedTypes = new HashSet<Type>();

        /// <summary>
        /// The set of types currently stored by this rule.
        /// </summary>
        protected HashSet<Type> StoredTypes
        {
            get => storedTypes;
        }

        /// <summary>
        /// Adds an additional Type to the list and returns a changed instance of this rule.
        /// </summary>
        /// <typeparam name="T">Type which is added.</typeparam>
        /// <returns>A new instance of this rule containing <typeparamref name="T"/> in addition to the already stored types.</returns>
        public TRecursive Add<T>() where T : TValueBase
        {
            TRecursive result = Clone();
            if (result.storedTypes.Contains(typeof(T)))
            {
                return result;
            }

            result.storedTypes.Add(typeof(T));
            return result;
        }

        /// <summary>
        /// Creates a copy of this rule that contains the same stored types.
        /// </summary>
        /// <returns>A new instance of this rule with a copied type set.</returns>
        protected virtual TRecursive Clone()
        {
            TRecursive result = new TRecursive { storedTypes = new HashSet<Type>(storedTypes) };
            return result;
        }
    }
}