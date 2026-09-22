// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.Serialization;

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Engine-agnostic, serializable representation of an animation curve.
    /// Stores the keyframes and wrap modes so curves can be recreated in any engine.
    /// </summary>
    [DataContract]
    public struct AnimationCurveData : IAnimationCurve
    {
        /// <summary>The keyframes that define the shape of the curve.</summary>
        [DataMember]
        public KeyframeData[] Keyframes { readonly get; set; }

        /// <summary>The wrap mode applied before the first keyframe, represented as an integer.</summary>
        [DataMember]
        public int PreWrapMode { readonly get; set; }

        /// <summary>The wrap mode applied after the last keyframe, represented as an integer.</summary>
        [DataMember]
        public int PostWrapMode { readonly get; set; }

        /// <summary>
        /// Initializes a new <see cref="AnimationCurveData"/> with the specified keyframes and wrap modes.
        /// </summary>
        /// <param name="keyframes">The keyframes that define the shape of the curve.</param>
        /// <param name="preWrapMode">The wrap mode applied before the first keyframe. Defaults to 0.</param>
        /// <param name="postWrapMode">The wrap mode applied after the last keyframe. Defaults to 0.</param>
        public AnimationCurveData(KeyframeData[] keyframes, int preWrapMode = 0, int postWrapMode = 0)
        {
            Keyframes = keyframes;
            PreWrapMode = preWrapMode;
            PostWrapMode = postWrapMode;
        }

        /// <summary>
        /// Creates a linear animation curve between two points.
        /// </summary>
        /// <param name="timeStart">The time of the first keyframe.</param>
        /// <param name="valueStart">The value of the first keyframe.</param>
        /// <param name="timeEnd">The time of the second keyframe.</param>
        /// <param name="valueEnd">The value of the second keyframe.</param>
        /// <returns>A new <see cref="AnimationCurveData"/> containing the two keyframes.</returns>
        public static AnimationCurveData Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
        {
            return new AnimationCurveData
            {
                Keyframes = new[]
                {
                    new KeyframeData(timeStart, valueStart),
                    new KeyframeData(timeEnd, valueEnd)
                }
            };
        }
    }
}