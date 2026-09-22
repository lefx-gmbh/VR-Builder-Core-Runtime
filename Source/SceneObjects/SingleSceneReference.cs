using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VRBuilder.Core.Runtime.Registry;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Step inspector reference to a single object.
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class SingleSceneReference<T> : ProcessSceneReference<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="SingleSceneReference{T}"/> referencing no objects.
        /// </summary>
        public SingleSceneReference() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SingleSceneReference{T}"/> referencing the object with the given guid.
        /// </summary>
        /// <param name="guid">The guid of the object this reference should point to.</param>
        public SingleSceneReference(Guid guid) : base(guid)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SingleSceneReference{T}"/> with the given set of guids.
        /// </summary>
        /// <param name="guids">The guids this reference should point to.</param>
        public SingleSceneReference(IEnumerable<Guid> guids) : base(guids)
        {
        }

        /// <summary>
        /// The single object referenced by this scene reference, or <c>null</c> when the reference is empty or unresolved.
        /// </summary>
        public T? Value
        {
            get { return DetermineValue(null); }
        }

        /// <inheritdoc />
        internal override bool AllowMultipleValues => false;

        /// <inheritdoc />
        public override bool HasValue()
        {
            return IsEmpty() == false && Value != null;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Guids.Count == 1 && ServiceRegistry.Get<ISceneObjectRegistry>().SceneObjectGroups.GroupExists(Guids.First()))
            {
                return $"an object in '{ServiceRegistry.Get<ISceneObjectRegistry>().SceneObjectGroups.GetLabel(Guids.First())}'";
            }

            if (HasValue() == false)
            {
                return "[NULL]";
            }

            return $"'{Value}'";
        }

        /// <summary>
        /// Determine the object referenced by this scene reference.
        /// </summary>
        protected abstract T? DetermineValue(T? cachedValue);
    }
}