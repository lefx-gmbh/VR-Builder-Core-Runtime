// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Behavior that waits for `DelayTime` seconds before finishing its activation.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/delay-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class DelayBehavior : Behavior<DelayBehavior.EntityData>
    {
        /// <summary>
        /// Creates a new <see cref="DelayBehavior"/> with no delay.
        /// </summary>
        [JsonConstructor]
        public DelayBehavior() : this(0)
        {
        }

        /// <summary>
        /// Creates a behavior that waits <paramref name="delayTime"/> seconds before completing.
        /// Negative values are clamped to zero.
        /// </summary>
        /// <param name="delayTime">Delay in seconds; must be zero or positive.</param>
        public DelayBehavior(float delayTime)
        {
            if (delayTime < 0f)
            {
                ForwardingLogger.LogWarningFormat("DelayTime has to be zero or positive, but it was {0}. Setting to 0 instead.", delayTime);
                delayTime = 0f;
            }

            Data.DelayTime = delayTime;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <summary>
        /// The data class for a delay behavior.
        /// </summary>
        [DisplayName("Delay")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Delay in seconds before the behavior completes.
            /// </summary>
            [DataMember]
            [DisplayName("Delay")]
            [DisplayTooltip("Delay before the behavior completes, in seconds.")]
            public float DelayTime { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get { return $"Wait for {DelayTime} seconds"; }
            }
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
                while (stopWatch.ElapsedMilliseconds < Data.DelayTime)
                    yield return null;
            }

            /// <inheritdoc />
            public override void End()
            {
                stopWatch.Stop();
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }
    }
}