// Copyright (c) 2021-2026 MindPort GmbH
// Licensed under the Apache License, Version 2.0

using System;

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Simple concrete implementation of <see cref="IColor"/> for use in CoreRuntime.
    /// Conversion to/from UnityEngine.Color happens in the Core layer via extension methods.
    /// </summary>
    public struct ColorData : IColor, IEquatable<ColorData>
    {
        /// <inheritdoc/>
        public float R { get; }

        /// <inheritdoc/>
        public float G { get; }

        /// <inheritdoc/>
        public float B { get; }

        /// <inheritdoc/>
        public float A { get; }

        /// <summary>
        /// Creates a new ColorData from float components (0..1 range).
        /// </summary>
        /// <param name="r">Red component in the 0..1 range.</param>
        /// <param name="g">Green component in the 0..1 range.</param>
        /// <param name="b">Blue component in the 0..1 range.</param>
        /// <param name="a">Alpha component in the 0..1 range. Defaults to 1.</param>
        public ColorData(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <inheritdoc/>
        public bool Equals(ColorData other)
        {
            return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B) && A.Equals(other.A);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ColorData other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        /// <summary>Returns <c>true</c> if <paramref name="left"/> and <paramref name="right"/> represent the same color.</summary>
        public static bool operator ==(ColorData left, ColorData right) => left.Equals(right);

        /// <summary>Returns <c>true</c> if <paramref name="left"/> and <paramref name="right"/> represent different colors.</summary>
        public static bool operator !=(ColorData left, ColorData right) => !left.Equals(right);

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ColorData({R:F3}, {G:F3}, {B:F3}, {A:F3})";
        }
    }
}