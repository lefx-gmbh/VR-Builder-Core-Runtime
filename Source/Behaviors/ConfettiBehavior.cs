// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Behavior that spawns a confetti machine and emits confetti for the configured duration.
    /// Confetti is spawned either above the user or at a position provider; the machine is loaded
    /// from a prefab path, activated on the configured execution stages, and cleaned up when done.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/spawn-confetti-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class ConfettiBehavior : Behavior<ConfettiBehavior.EntityData>
    {
        private const float defaultDuration = 15f;
        private const float defaultRadius = 1f;
        private const float distanceAboveUser = 3f;

        /// <summary>
        /// Creates a confetti behavior with default values.
        /// </summary>
        [JsonConstructor]
        public ConfettiBehavior() : this(true, Guid.Empty, "", defaultRadius, defaultDuration, BehaviorExecutionStages.Activation)
        {
        }

        /// <summary>
        /// Creates a confetti behavior that spawns confetti at the given position provider.
        /// </summary>
        /// <param name="isAboveUser">If <c>true</c>, confetti is spawned above the user instead of at the position provider.</param>
        /// <param name="positionProvider">The object where the confetti machine is spawned.</param>
        /// <param name="confettiMachinePrefabPath">Path to the confetti machine prefab.</param>
        /// <param name="radius">Radius of the spawning area.</param>
        /// <param name="duration">Duration of the confetti emission in seconds.</param>
        /// <param name="executionStages">The stages during which the confetti is emitted.</param>
        public ConfettiBehavior(bool isAboveUser, ISceneObject positionProvider, string confettiMachinePrefabPath, float radius, float duration, BehaviorExecutionStages executionStages)
            : this(isAboveUser, ProcessReferenceUtils.GetUniqueIdFrom(positionProvider), confettiMachinePrefabPath, radius, duration, executionStages)
        {
        }

        /// <summary>
        /// Creates a confetti behavior that spawns confetti at the object with the given unique id.
        /// </summary>
        /// <param name="isAboveUser">If <c>true</c>, confetti is spawned above the user instead of at the position provider.</param>
        /// <param name="positionProviderId">The unique id of the object where the confetti machine is spawned.</param>
        /// <param name="confettiMachinePrefabPath">Path to the confetti machine prefab.</param>
        /// <param name="radius">Radius of the spawning area.</param>
        /// <param name="duration">Duration of the confetti emission in seconds.</param>
        /// <param name="executionStages">The stages during which the confetti is emitted.</param>
        public ConfettiBehavior(bool isAboveUser, Guid positionProviderId, string confettiMachinePrefabPath, float radius, float duration, BehaviorExecutionStages executionStages)
        {
            Data.IsAboveUser = isAboveUser;
            Data.ConfettiPosition = new SingleScenePropertyReference<IEffectProperty>(positionProviderId);
            Data.ConfettiMachinePrefabPath = confettiMachinePrefabPath;
            Data.AreaRadius = radius;
            Data.Duration = duration;
            Data.ExecutionStages = executionStages;

#if UNITY_6000_0_OR_NEWER
            if (string.IsNullOrEmpty(Data.ConfettiMachinePrefabPath) && ServiceRegistry.Has<ISceneService>())
            {
                Data.ConfettiMachinePrefabPath = ServiceRegistry.Get<ISceneService>().DefaultConfettiPrefab;
            }
#endif
        }


        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new EmitConfettiProcess(Data, BehaviorExecutionStages.Activation);
        }

        /// <inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new EmitConfettiProcess(Data, BehaviorExecutionStages.Deactivation);
        }

        /// <summary>
        /// The data for a <see cref="ConfettiBehavior"/>.
        /// </summary>
        [DisplayName("Spawn Confetti")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData, IBehaviorExecutionStages
        {
            /// <summary>
            /// Bool to check whether the confetti machine should spawn above the user or at the position of the position provider.
            /// </summary>
            [DataMember]
            [DisplayName("Spawn Above User")]
            public bool IsAboveUser { get; set; }

            /// <summary>
            /// Name of the process object where to spawn the confetti machine.
            /// Only needed if "Spawn Above User" is not checked.
            /// </summary>
            [DataMember]
            [DisplayName("Position Provider")]
            public SingleScenePropertyReference<IEffectProperty> ConfettiPosition { get; set; }

            /// <summary>
            /// Path to the desired confetti machine prefab.
            /// </summary>
            [DataMember]
            [DisplayName("Confetti Machine Path")]
            [UsesSpecificProcessDrawer("ConfettiMachinePathDrawer")]
            public string ConfettiMachinePrefabPath { get; set; }

            /// <summary>
            /// Radius of the spawning area.
            /// </summary>
            [DataMember]
            [DisplayName("Area Radius")]
            public float AreaRadius { get; set; }

            /// <summary>
            /// Duration of the animation in seconds.
            /// </summary>
            [DataMember]
            [DisplayName("Duration")]
            public float Duration { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string positionProvider = "user";
                    if (IsAboveUser == false)
                    {
                        positionProvider = ConfettiPosition.HasValue() ? $"{ConfettiPosition.Value}" : "[NULL]";
                    }

                    return $"Spawn confetti on {positionProvider}";
                }
            }

            /// <inheritdoc />
            [DataMember]
            [DisplayName("Execution Stages")]
            [DisplayTooltip("Determines whether the behavior runs when the step activates, deactivates, or both.")]
            public BehaviorExecutionStages ExecutionStages { get; set; }
        }

        private class EmitConfettiProcess : StageProcess<EntityData>
        {
            private readonly BehaviorExecutionStages stages;
            private readonly Stopwatch stopWatch = new();

            public EmitConfettiProcess(EntityData data, BehaviorExecutionStages stages) : base(data)
            {
                this.stages = stages;
            }

            /// <inheritdoc />
            public override void Start()
            {
                if (ShouldExecuteCurrentStage(Data) == false)
                {
                    return;
                }

                var loadSucceeded = Data.ConfettiPosition.Value.LoadConfettiMachinePrefab(Data.ConfettiMachinePrefabPath);

                // Load the given prefab and stop the coroutine if not possible.

                if (!loadSucceeded)
                {
                    ForwardingLogger.LogWarning("No valid prefab path provided.");
                    return;
                }

                if (Data.IsAboveUser)
                {
                    Data.ConfettiPosition.Value.CreateConfettiMachineAboveUser();
                }
                else
                {
                    Data.ConfettiPosition.Value.ConfettiMachineCreatedAction += OnConfettiMachineCreated;
                    Data.ConfettiPosition.Value.CreateConfettiMachine();
                }

                if (Data.Duration > 0f)
                {
                    stopWatch.Restart();
                }
            }

            private void OnConfettiMachineCreated()
            {
                Data.ConfettiPosition.Value.ActivateConfettiMachine(Data.AreaRadius, Data.Duration);
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                if (ShouldExecuteCurrentStage(Data) == false)
                {
                    yield break;
                }

                if (Data.ConfettiPosition.Value.Count == 0)
                {
                    yield break;
                }

                if (Data.Duration > 0)
                {
                    while (stopWatch.ElapsedMilliseconds < Data.Duration)
                    {
                        yield return null;
                    }
                }
            }

            /// <inheritdoc />
            public override void End()
            {
                if (ShouldExecuteCurrentStage(Data))
                {
                    Data.ConfettiPosition.Value.ClearConfettiMachines();
                    Data.ConfettiPosition.Value.ConfettiMachineCreatedAction -= OnConfettiMachineCreated;
                }

                stopWatch.Stop();
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }

            private bool ShouldExecuteCurrentStage(EntityData data)
            {
                return (data.ExecutionStages & stages) > 0;
            }
        }
    }
}