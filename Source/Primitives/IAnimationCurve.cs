// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Engine-agnostic representation of an animation curve (a piecewise function over time),
    /// used to share keyframe and wrapping data between Unity's AnimationCurve and Godot's Curve.
    /// </summary>
    public interface IAnimationCurve
    {
        /// <summary>Gets or sets the ordered keyframes defining the curve's control points.</summary>
        KeyframeData[] Keyframes { get; set; }

        /// <summary>
        /// Gets or sets the behavior of the curve before its first keyframe, encoded as an
        /// engine-specific wrap-mode value.
        /// </summary>
        int PreWrapMode { get; set; }

        /// <summary>
        /// Gets or sets the behavior of the curve after its last keyframe, encoded as an
        /// engine-specific wrap-mode value.
        /// </summary>
        int PostWrapMode { get; set; }
    }
}