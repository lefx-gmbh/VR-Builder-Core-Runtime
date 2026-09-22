// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;

namespace VRBuilder.Core.Conditions
{
    /// <summary>
    /// A condition that completes when a certain amount of time has passed.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/timeout-condition.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class TimeoutCondition : Condition<TimeoutCondition.EntityData>
    {
        /// <summary>
        /// Creates an empty timeout condition, used by the JSON deserializer.
        /// </summary>
        [JsonConstructor]
        public TimeoutCondition() : this(0)
        {
        }

        /// <summary>
        /// Creates a timeout condition that completes after <paramref name="timeout"/> seconds.
        /// </summary>
        /// <param name="timeout">Delay before the condition completes, in seconds.</param>
        public TimeoutCondition(float timeout)
        {
            Data.Timeout = timeout;
        }

        /// <inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        /// <summary>
        /// The data for timeout condition.
        /// </summary>
        [DisplayName("Timeout")]
        public class EntityData : IConditionData
        {
            /// <summary>
            /// The delay before the condition completes.
            /// </summary>
            [DataMember]
            [DisplayName("Wait")]
            [DisplayTooltip("Delay before the condition completes, in seconds.")]
            public float Timeout { get; set; }

            /// <summary>
            /// True if the configured timeout has elapsed.
            /// </summary>
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            [HideInProcessInspector]
            public string Name
            {
                get { return $"Complete after {Timeout} seconds"; }
            }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : BaseActiveProcessOverCompletable<EntityData>
        {
            private readonly Stopwatch stopWatch = new();

            public ActiveProcess(EntityData data) : base(data)
            {
            }

            public override void Start()
            {
                base.Start();
                stopWatch.Restart();
            }

            /// <inheritdoc />
            protected override bool CheckIfCompleted()
            {
                return stopWatch.ElapsedMilliseconds >= Data.Timeout;
            }

            /// <inheritdoc />
            public override void End()
            {
                base.End();
                stopWatch.Stop();
            }
        }
    }
}