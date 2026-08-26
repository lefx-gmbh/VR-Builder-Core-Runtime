using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Base class for a process reference to one or more objects.
    /// </summary>
    [DataContract(IsReference = true)]
    public abstract class ProcessSceneReferenceBase : ICanBeEmpty
    {
        [DataMember]
        private List<Guid> guids;

        /// <summary>
        /// Initializes a new instance of <see cref="ProcessSceneReferenceBase"/> with an empty set of guids.
        /// </summary>
        public ProcessSceneReferenceBase()
        {
            guids = new List<Guid>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProcessSceneReferenceBase"/> referencing the object with the given guid.
        /// </summary>
        /// <param name="guid">The guid this reference should point to. If <see cref="Guid.Empty"/>, the reference is initialized without any guids.</param>
        public ProcessSceneReferenceBase(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                guids = new List<Guid>();
            }
            else
            {
                guids = new List<Guid> { guid };
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProcessSceneReferenceBase"/> with the given set of guids.
        /// </summary>
        /// <param name="guids">The guids this reference should point to. If <c>null</c>, the reference is initialized without any guids.</param>
        public ProcessSceneReferenceBase(IEnumerable<Guid> guids)
        {
            if (guids == null)
            {
                this.guids = new List<Guid>();
            }
            else
            {
                this.guids = guids.ToList();
            }
        }

        /// <summary>
        /// List of guids, each Guid is a reference to a <see cref="Settings.SceneObjectGroups.SceneObjectGroup"/>.
        /// </summary>
        public IReadOnlyList<Guid> Guids => guids;

        /// <summary>
        /// If true, this reference can return multiple values.
        /// </summary>
        internal abstract bool AllowMultipleValues { get; }

        /// <inheritdoc />
        public virtual bool IsEmpty()
        {
            return Guids == null || Guids.Count() == 0 || Guids.All(guid => guid == Guid.Empty);
        }

        /// <summary>
        /// Returns the type this reference is associated with.
        /// </summary>        
        internal abstract Type GetReferenceType();

        /// <summary>
        /// Returns true if the reference contains a non-null value.
        /// </summary>        
        public abstract bool HasValue();

        /// <summary>
        /// Adds the specified guid to this reference.
        /// </summary>
        /// <param name="guid">The guid to add to the reference's set of referenced guids.</param>
        public void AddGuid(Guid guid)
        {
            guids.Add(guid);
        }

        /// <summary>
        /// Removes the specified guid from this reference.
        /// </summary>
        /// <param name="guid">The guid to remove from the reference's set of referenced guids.</param>
        /// <returns><c>true</c> if the guid was present and removed; otherwise, <c>false</c>.</returns>
        public bool RemoveGuid(Guid guid)
        {
            return guids.Remove(guid);
        }

        /// <summary>
        /// Resets the guids on this reference to the specified value.
        /// </summary>
        /// <param name="newGuids">The guids to replace the current set with. If <c>null</c>, the reference is cleared.</param>
        public void ResetGuids(IEnumerable<Guid>? newGuids = null)
        {
            if (newGuids == null)
            {
                guids.Clear();
            }
            else
            {
                guids = newGuids.ToList();
            }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ProcessSceneReferenceBase sceneReference = obj as ProcessSceneReferenceBase;

            if (sceneReference == null)
            {
                return false;
            }

            return GetType() == sceneReference.GetType() &&
                   Guids.OrderBy(guid => guid).SequenceEqual(sceneReference.Guids.OrderBy(guid => guid)) &&
                   AllowMultipleValues == sceneReference.AllowMultipleValues;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(GetType());
            hashCode.Add(AllowMultipleValues);

            foreach (Guid guid in Guids.OrderBy(guid => guid))
            {
                hashCode.Add(guid);
            }

            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Determines whether two process scene references are equal.
        /// </summary>
        /// <param name="left">The first reference to compare.</param>
        /// <param name="right">The second reference to compare.</param>
        /// <returns><c>true</c> if both references are <c>null</c>, or if <paramref name="left"/> equals <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(ProcessSceneReferenceBase? left, ProcessSceneReferenceBase? right)
        {
            if ((object)left == null)
            {
                return (object)right == null;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two process scene references are not equal.
        /// </summary>
        /// <param name="left">The first reference to compare.</param>
        /// <param name="right">The second reference to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(ProcessSceneReferenceBase? left, ProcessSceneReferenceBase? right)
        {
            if ((object)left == null)
            {
                return (object)right != null;
            }

            return left.Equals(right) == false;
        }
    }
}