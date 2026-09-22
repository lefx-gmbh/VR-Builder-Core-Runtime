using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VRBuilder.Core.Runtime.Registry;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Step inspector reference to multiple objects.
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class MultipleSceneReference<T> : ProcessSceneReference<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultipleSceneReference{T}"/> referencing no objects.
        /// </summary>
        public MultipleSceneReference() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MultipleSceneReference{T}"/> referencing the object with the given guid.
        /// </summary>
        /// <param name="guid">The guid of the object this reference should point to.</param>
        public MultipleSceneReference(Guid guid) : base(guid)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MultipleSceneReference{T}"/> with the given set of guids.
        /// </summary>
        /// <param name="guids">The guids this reference should point to.</param>
        public MultipleSceneReference(IEnumerable<Guid> guids) : base(guids)
        {
        }

        /// <summary>
        /// The referenced values.
        /// </summary>
        public IEnumerable<T> Values
        {
            get { return DetermineValue(null); }
        }

        /// <inheritdoc/>
        internal override bool AllowMultipleValues => true;

        /// <summary>
        /// Determine the objects referenced by this scene reference.
        /// </summary>
        protected abstract IEnumerable<T> DetermineValue(IEnumerable<T> cachedValue);

        /// <inheritdoc/>
        public override bool HasValue()
        {
            return IsEmpty() == false && Values != null;
        }

        /// <summary>
        /// Converts this scene reference into a list of the referenced values.
        /// </summary>
        /// <param name="reference">The scene reference whose referenced values should be returned.</param>
        /// <returns>A list containing all values referenced by <paramref name="reference"/>.</returns>
        public static implicit operator List<T>(MultipleSceneReference<T> reference)
        {
            return reference.Values.ToList();
        }

        /// <summary>
        /// Returns a human-readable description of the referenced objects.
        /// </summary>
        /// <returns>The group label when the reference points to a single group, <c>[NULL]</c> when no value is available,
        /// the referenced object's text when exactly one value is referenced, or a count of the referenced objects otherwise.</returns>
        public override string ToString()
        {
            if (Guids.Count == 1 && ServiceRegistry.Get<ISceneObjectRegistry>().SceneObjectGroups.GroupExists(Guids.First()))
            {
                return $"objects in '{ServiceRegistry.Get<ISceneObjectRegistry>().SceneObjectGroups.GetLabel(Guids.First())}'";
            }

            if (HasValue() == false)
            {
                return "[NULL]";
            }

            if (Values.Count() == 1)
            {
                return $"'{Values.First()}'";
            }

            return $"{Values.Count()} objects";
        }
    }
}