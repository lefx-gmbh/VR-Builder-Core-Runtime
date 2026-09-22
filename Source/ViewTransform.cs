using System.Runtime.Serialization;
using VRBuilder.Core.Primitives;

namespace VRBuilder.Core
{
    /// <summary>
    /// Stores position and scale in a viewport.
    /// </summary>
    [DataContract(IsReference = true)]
    public class ViewTransform
    {
        /// <summary>
        /// Creates a new <see cref="ViewTransform"/> with the given position and scale.
        /// </summary>
        /// <param name="position">The position in the viewport.</param>
        /// <param name="scale">The scale in the viewport.</param>
        public ViewTransform(IVector3 position, IVector3 scale)
        {
            Position = position;
            Scale = scale;
        }

        /// <summary>
        /// The position of the transform in the viewport.
        /// </summary>
        [DataMember]
        public IVector3 Position { get; set; }

        /// <summary>
        /// The scale of the transform in the viewport.
        /// </summary>
        [DataMember]
        public IVector3 Scale { get; set; }
    }
}