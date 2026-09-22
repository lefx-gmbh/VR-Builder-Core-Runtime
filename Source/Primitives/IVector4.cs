// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Engine-agnostic 4-component vector (X, Y, Z, W) used to transport vector data across
    /// engine boundaries without referencing Unity or Godot vector types.
    /// </summary>
    public interface IVector4
    {
        /// <summary>Gets the X component of the vector.</summary>
        float X { get; }

        /// <summary>Gets the Y component of the vector.</summary>
        float Y { get; }

        /// <summary>Gets the Z component of the vector.</summary>
        float Z { get; }

        /// <summary>Gets the W component of the vector.</summary>
        float W { get; }
    }
}