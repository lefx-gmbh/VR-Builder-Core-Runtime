// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Primitives;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Linearly changes the scale of the target objects over the duration until it matches the target scale.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/scale-objects-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class ScalingBehavior : Behavior<ScalingBehavior.EntityData>
    {
        /// <summary>
        /// Creates a scaling behavior with default values.
        /// </summary>
        [JsonConstructor]
        public ScalingBehavior() : this(Array.Empty<ISceneObject>(), Vector3Data.One, 0f)
        {
        }

        /// <summary>
        /// Creates a scaling behavior that scales the given objects to the target scale over the duration.
        /// </summary>
        /// <param name="targets">The objects to scale.</param>
        /// <param name="targetScale">The scale the objects should end at.</param>
        /// <param name="duration">The duration of the animation in seconds.</param>
        public ScalingBehavior(IEnumerable<ISceneObject> targets, IVector3 targetScale, float duration)
        {
            Data.Targets = new MultipleScenePropertyReference<IScaleProperty>(targets.Select(target => target.Guid));
            Data.TargetScale = targetScale;
            Data.Duration = duration;
            Data.AnimationCurve = AnimationCurveData.Linear(0f, 0f, 1f, 1f);
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <summary>
        /// The data for a <see cref="ScalingBehavior"/>.
        /// </summary>
        [DisplayName("Scale Object")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// The process objects to scale.
            /// </summary>
            [DataMember]
            [DisplayName("Target Objects")]
            public MultipleScenePropertyReference<IScaleProperty> Targets { get; set; }

            /// <summary>
            /// The target scale of the objects.
            /// </summary>
            [DataMember]
            [DisplayName("Target Scale")]
            public IVector3 TargetScale { get; set; }

            /// <summary>
            /// Duration of the animation in seconds.
            /// </summary>
            [DataMember]
            [DisplayName("Animation Duration")]
            [DisplayTooltip("Duration of the animation in seconds.")]
            public float Duration { get; set; }

            /// <summary>
            /// The curve used to interpolate the scale over time.
            /// </summary>
            [DataMember]
            [DisplayName("Animation curve")]
            public IAnimationCurve AnimationCurve { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name => $"Scale {Targets} to {TargetScale}";
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            private readonly Stopwatch stopWatch = new();

            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                stopWatch.Restart();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                if (!Data.Targets.HasValue())
                {
                    throw new InvalidOperationException("ScalingBehavior: No target objects assigned to scale.");
                }

                if (!Data.Targets.Values.Any())
                {
                    yield break;
                }

                while (stopWatch.ElapsedMilliseconds < Data.Duration)
                {
                    float progress = stopWatch.ElapsedMilliseconds / Data.Duration;

                    foreach (IScaleProperty property in Data.Targets.Values)
                    {
                        property.ScaleTo(Data.TargetScale, progress, Data.AnimationCurve);
                    }

                    yield return null;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                foreach (var property in Data.Targets.Values)
                {
                    property.ScaleTo(Data.TargetScale, 1f, null);
                }

                stopWatch.Stop();
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }
    }
}