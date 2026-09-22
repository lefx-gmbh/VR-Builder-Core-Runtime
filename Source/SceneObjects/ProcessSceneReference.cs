using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VRBuilder.Core.SceneObjects
{
    /// <summary>
    /// Step inspector reference to a scene object or a specific property.
    /// </summary>    
    [DataContract(IsReference = true)]
    public abstract class ProcessSceneReference<T> : ProcessSceneReferenceBase where T : class
    {
        /// <summary>
        /// Creates an empty scene reference.
        /// </summary>
        public ProcessSceneReference() : base()
        {
        }

        /// <summary>
        /// Creates a scene reference to the object or property with the given unique id.
        /// </summary>
        /// <param name="guid">The unique id of the referenced object or property.</param>
        public ProcessSceneReference(Guid guid) : base(guid)
        {
        }

        /// <summary>
        /// Creates a scene reference to the objects or properties with the given unique ids.
        /// </summary>
        /// <param name="guids">The unique ids of the referenced objects or properties.</param>
        public ProcessSceneReference(IEnumerable<Guid> guids) : base(guids)
        {
        }

        /// <inheritdoc />
        internal override Type GetReferenceType()
        {
            return typeof(T);
        }
    }
}