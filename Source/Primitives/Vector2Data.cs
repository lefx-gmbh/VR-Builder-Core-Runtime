// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.Serialization;
using VRBuilder.Core.Primitives;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Engine-agnostic, serializable 2D vector struct implementing <see cref="IVector2"/>,
    /// used to transport vector data across engine boundaries and persist it via
    /// data-contract serialization.
    /// </summary>
    [DataContract]
    public struct Vector2Data : IVector2
    {
        /// <summary>Gets or sets the X component of the vector.</summary>
        [DataMember]
        public float X { readonly get; set; }

        /// <summary>Gets or sets the Y component of the vector.</summary>
        [DataMember]
        public float Y { readonly get; set; }

        /// <summary>Initializes a new instance with the given X and Y components.</summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        public Vector2Data(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}