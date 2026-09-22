// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;

namespace VRBuilder.Core.Configuration.Modes
{
    /// <summary>
    /// An <see cref="IRule{T}"/> for types.
    /// </summary>
    /// <typeparam name="TValueBase">A type from which all input types have to be inherited from.</typeparam>
    public abstract class TypeRule<TValueBase> : IRule<Type>
    {
        /// <summary>
        /// Generic version of <see cref="IsQualifiedBy"/>.
        /// </summary>
        public bool IsQualifiedBy<T>() where T : TValueBase
        {
            return IsQualifiedBy(typeof(T));
        }

        /// <inheritdoc />
        public bool IsQualifiedBy(Type type)
        {
            return typeof(TValueBase).IsAssignableFrom(type) && IsQualifiedByPredicate(type);

        }

        /// <summary>
        /// Actual check of a given type. It is guaranteed that <paramref name="type"/> inherits the <typeparam name="TValueBase" />
        /// </summary>
        protected abstract bool IsQualifiedByPredicate(Type type);
    }
}
