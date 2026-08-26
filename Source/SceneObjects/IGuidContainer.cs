using System;
using System.Collections.Generic;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// A container for a list of guids that are associated to an object.
    /// </summary>
    public interface IGuidContainer
    {
        /// <summary>
        /// All guids on the object.
        /// </summary>
        IEnumerable<Guid> Guids { get; }

        /// <summary>
        /// Raised when a guid is added.
        /// </summary>
        event Action<object, IGuidContainerEventArgs> GuidAddedAction;

        /// <summary>
        /// Raised when a guid is removed.
        /// </summary>
        event Action<object, IGuidContainerEventArgs> GuidRemovedAction;

        /// <summary>
        /// True if the object has the specified guid.
        /// </summary>
        bool HasGuid(Guid gd);

        /// <summary>
        /// Add the specified guid.
        /// </summary>        
        void AddGuid(Guid gd);

        /// <summary>
        /// Remove the specified guid.
        /// </summary>
        bool RemoveGuid(Guid gd);
    }

    public interface IGuidContainerEventArgs
    {
        public Guid Guid { get; }
    }
}