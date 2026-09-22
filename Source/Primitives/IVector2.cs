// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// A two-dimensional vector with float components.
    /// </summary>
    public interface IVector2
    {
        /// <summary>
        /// The x component of the vector.
        /// </summary>
        float X { get; }

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        float Y { get; }
    }
}