// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Read-only description of a keyframe on an animation curve.
    /// Implemented by engine-agnostic data types so curve definitions stay Unity-free.
    /// </summary>
    public interface IKeyframe
    {
        /// <summary>The time at which the keyframe occurs on the curve.</summary>
        float Time { get; }

        /// <summary>The value of the curve at <see cref="Time"/>.</summary>
        float Value { get; }

        /// <summary>The tangent of the curve entering the keyframe.</summary>
        float InTangent { get; }

        /// <summary>The tangent of the curve leaving the keyframe.</summary>
        float OutTangent { get; }

        /// <summary>The weighted tangent mode of the keyframe, represented as an integer.</summary>
        int WeightedMode { get; set; }
    }
}