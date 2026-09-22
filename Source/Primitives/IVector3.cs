// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Engine-agnostic 3-component vector (X, Y, Z) used to transport vector data across
    /// engine boundaries without referencing Unity or Godot vector types.
    /// </summary>
    public interface IVector3
    {
        /// <summary>Gets the X component of the vector.</summary>
        float X { get; }

        /// <summary>Gets the Y component of the vector.</summary>
        float Y { get; }

        /// <summary>Gets the Z component of the vector.</summary>
        float Z { get; }
    }
}