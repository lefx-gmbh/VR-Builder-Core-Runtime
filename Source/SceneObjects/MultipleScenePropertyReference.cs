// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Runtime.Registry;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Step inspector reference to multiple <see cref="ISceneObjectProperty"/>s.
    /// </summary>    
    [DataContract(IsReference = true)]
    public class MultipleScenePropertyReference<T> : MultipleSceneReference<T> where T : class, ISceneObjectProperty
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultipleScenePropertyReference{T}"/> referencing no properties.
        /// </summary>
        public MultipleScenePropertyReference() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MultipleScenePropertyReference{T}"/> referencing the property with the given guid.
        /// </summary>
        /// <param name="guid">The guid of the property this reference should point to.</param>
        public MultipleScenePropertyReference(Guid guid) : base(guid)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MultipleScenePropertyReference{T}"/> with the given set of guids.
        /// </summary>
        /// <param name="guids">The guids this reference should point to.</param>
        public MultipleScenePropertyReference(IEnumerable<Guid> guids) : base(guids)
        {
        }

        /// <inheritdoc/>
        protected override IEnumerable<T> DetermineValue(IEnumerable<T> cachedValue)
        {
            if (!ServiceRegistry.Has<IRuntimeService>() || IsEmpty())
            {
                return new List<T>();
            }

            IEnumerable<T> value = cachedValue;

            // If value exists, return it.
            if (value != null)
            {
                return value;
            }

            value = new List<T>();

            foreach (Guid guid in Guids)
            {
                value = value.Concat(ServiceRegistry.Get<ISceneObjectRegistry>().GetProperties<T>(guid)).Distinct();
            }

            return value;
        }
    }
}