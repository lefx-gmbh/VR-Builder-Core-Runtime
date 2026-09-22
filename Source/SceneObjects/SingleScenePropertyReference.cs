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
    /// Step inspector reference to a single <see cref="ISceneObjectProperty"/>.
    /// </summary>
    [DataContract(IsReference = true)]
    public class SingleScenePropertyReference<T> : SingleSceneReference<T> where T : class, ISceneObjectProperty
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SingleScenePropertyReference{T}"/> referencing no properties.
        /// </summary>
        public SingleScenePropertyReference() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SingleScenePropertyReference{T}"/> referencing the property with the given guid.
        /// </summary>
        /// <param name="guid">The guid of the property this reference should point to.</param>
        public SingleScenePropertyReference(Guid guid) : base(guid)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SingleScenePropertyReference{T}"/> with the given set of guids.
        /// </summary>
        /// <param name="guids">The guids this reference should point to.</param>
        public SingleScenePropertyReference(IEnumerable<Guid> guids) : base(guids)
        {
        }

        /// <inheritdoc />
        protected override T? DetermineValue(T? cachedValue)
        {
            if (!ServiceRegistry.Has<IRuntimeService>() || IsEmpty())
            {
                return null;
            }

            T value = cachedValue;

            // If MonoBehaviour was destroyed, nullify the value.
            if (value != null && value.Equals(null))
            {
                value = null;
            }

            // If value exists, return it.
            if (value != null)
            {
                return value;
            }

            IEnumerable<T> properties = new List<T>();

            foreach (Guid guid in Guids)
            {
                properties = properties.Concat(ServiceRegistry.Get<ISceneObjectRegistry>().GetProperties<T>(guid)).Distinct();
            }

            return properties.FirstOrDefault();
        }
    }
}