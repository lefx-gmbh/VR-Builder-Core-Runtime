// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VRBuilder.Core.Serialization;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.EntityOwners;
using VRBuilder.Core.EntityOwners.ParallelEntityCollection;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Behavior that executes a stored chapter and completes when the chapter ends.
    /// </summary>
    [DataContract(IsReference = true)]
    public class ExecuteChapterBehavior : Behavior<ExecuteChapterBehavior.EntityData>
    {
        /// <summary>
        /// Creates a new <see cref="ExecuteChapterBehavior"/> without a chapter assigned.
        /// </summary>
        [SerializationConstructor]
        public ExecuteChapterBehavior() : this(null)
        {
        }

        /// <summary>
        /// Creates a behavior that executes <paramref name="chapter"/> as a step group.
        /// </summary>
        /// <param name="chapter">Chapter to execute; the behavior completes when it ends.</param>
        public ExecuteChapterBehavior(IChapter chapter)
        {
            Data.Chapter = chapter;
            Data.Name = "Step Group";
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new DeactivatingProcess(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetAbortingProcess()
        {
            return new ParallelAbortingProcess<EntityData>(Data);
        }

        /// <summary>
        /// Execute chapter behavior data.
        /// </summary>
        [DisplayName("Step Group")]
        [DataContract(IsReference = true)]
        public class EntityData : EntityCollectionData<IChapter>, IBehaviorData
        {
            /// <summary>
            /// Chapter executed as a step group; the behavior completes when this chapter ends.
            /// </summary>
            [DataMember]
            [DisplayName("Chapter")]
            [DisplayTooltip("Chapter executed as a step group.")]
            public IChapter Chapter { get; set; }

            /// <inheritdoc />
            public string Name { get; set; }

            /// <inheritdoc />
            public override IEnumerable<IChapter> GetChildren()
            {
                return new List<IChapter>() { Chapter };
            }
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                Data.Chapter.LifeCycle.Activate();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                while (Data.Chapter.LifeCycle.Stage != Stage.Active)
                {
                    Data.Chapter.Update();
                    yield return null;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
            }

            /// <inheritdoc />
            public override void FastForward()
            {
                if (Data.Chapter.Data.Current == null)
                {
                    Data.Chapter.Data.Current = Data.Chapter.Data.FirstStep;
                }

                Data.Chapter.LifeCycle.MarkToFastForwardStage(Stage.Activating);
            }
        }

        private class DeactivatingProcess : StageProcess<EntityData>
        {
            public DeactivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                Data.Chapter.LifeCycle.Deactivate();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                while (Data.Chapter.LifeCycle.Stage != Stage.Inactive)
                {
                    Data.Chapter.Update();
                    yield return null;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
            }

            /// <inheritdoc />
            public override void FastForward()
            {
                Data.Chapter.LifeCycle.MarkToFastForwardStage(Stage.Deactivating);
            }
        }
    }
}