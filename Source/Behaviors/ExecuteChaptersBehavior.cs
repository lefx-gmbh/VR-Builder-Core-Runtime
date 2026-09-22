// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.EntityOwners;
using VRBuilder.Core.EntityOwners.ParallelEntityCollection;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Behavior that executes a number of chapters at the same time and completes when the chapters ends.
    /// </summary>
    [DataContract(IsReference = true)]
    public class ExecuteChaptersBehavior : Behavior<ExecuteChaptersBehavior.EntityData>
    {
        /// <summary>
        /// Creates an execute-chapters behavior with no chapters.
        /// </summary>
        [JsonConstructor]
        public ExecuteChaptersBehavior() : this(chapters: new List<IChapter>())
        {
        }

        /// <summary>
        /// Creates an execute-chapters behavior that runs the given sub-chapters in parallel.
        /// </summary>
        /// <param name="subChapters">The sub-chapters to execute.</param>
        public ExecuteChaptersBehavior(IEnumerable<SubChapter> subChapters)
        {
            Data.SubChapters = new List<SubChapter>(subChapters);
        }

        /// <summary>
        /// Creates an execute-chapters behavior that runs the given chapters in parallel.
        /// </summary>
        /// <param name="chapters">The chapters to execute.</param>
        public ExecuteChaptersBehavior(IEnumerable<IChapter> chapters) : this(new List<SubChapter>(chapters.Select(chapter => new SubChapter(chapter))))
        {
        }

        /// <summary>
        /// Creates an execute-chapters behavior that runs the given chapter.
        /// </summary>
        /// <param name="chapter">The chapter to execute.</param>
        public ExecuteChaptersBehavior(IChapter chapter) : this(new List<SubChapter>() { new SubChapter(chapter) })
        {
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
        /// Execute chapters behavior data.
        /// </summary>
        [DisplayName("Execute Chapters")]
        [DataContract(IsReference = true)]
        public class EntityData : EntityCollectionData<IChapter>, IBehaviorData
        {
            private List<SubChapter> subChapters;

            /// <summary>
            /// SubChapters to be executed in parallel.
            /// </summary>
            [DataMember]
            [DisplayName("Sub Chapters")]
            [DisplayTooltip("Chapters to execute in parallel.")]
            public List<SubChapter> SubChapters { get; set; }

            /// <summary>
            /// If true, the chapter with the corresponding index can be interrupted
            /// if all other chapters are complete.
            /// </summary>
            [DataMember]
            [DisplayName("Is Optional Chapter")]
            [DisplayTooltip("If true, the chapter at the same index can be interrupted once all other chapters are complete.")]
            public List<bool> IsOptionalChapter { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name => "Execute Chapters";

            /// <inheritdoc />
            public override IEnumerable<IChapter> GetChildren()
            {
                return SubChapters.Select(sc => sc.Chapter);
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
                foreach (SubChapter subChapter in Data.SubChapters)
                {
                    subChapter.Chapter.LifeCycle.Activate();
                }
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                while (Data.SubChapters.Any(sc => sc.IsOptional == false && sc.Chapter.LifeCycle.Stage != Stage.Active))
                {
                    foreach (SubChapter sc in Data.SubChapters.Where(sc => sc.Chapter.LifeCycle.Stage == Stage.Activating))
                    {
                        sc.Chapter.Update();
                    }

                    yield return null;
                }

                foreach (SubChapter subChapter in Data.SubChapters.Where(sc => sc.IsOptional && sc.Chapter.LifeCycle.Stage == Stage.Activating))
                {
                    subChapter.Chapter.LifeCycle.Abort();
                }

                while (Data.SubChapters.Any(sc => sc.Chapter.LifeCycle.Stage == Stage.Aborting))
                {
                    foreach (SubChapter sc in Data.SubChapters.Where(sc => sc.Chapter.LifeCycle.Stage == Stage.Aborting))
                    {
                        sc.Chapter.Update();
                    }

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
                foreach (SubChapter subChapter in Data.SubChapters)
                {
                    if (subChapter.Chapter.Data.Current == null)
                    {
                        subChapter.Chapter.Data.Current = subChapter.Chapter.Data.FirstStep;
                    }

                    subChapter.Chapter.LifeCycle.MarkToFastForwardStage(Stage.Activating);
                }
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
                foreach (SubChapter subChapter in Data.SubChapters.Where(sc => sc.Chapter.LifeCycle.Stage != Stage.Inactive && sc.Chapter.LifeCycle.Stage != Stage.Aborting))
                {
                    subChapter.Chapter.LifeCycle.Deactivate();
                }
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                while (Data.SubChapters.Any(sc => sc.IsOptional == false && sc.Chapter.LifeCycle.Stage != Stage.Inactive))
                {
                    foreach (SubChapter subChapter in Data.SubChapters.Where(sc => sc.Chapter.LifeCycle.Stage != Stage.Inactive))
                    {
                        subChapter.Chapter.Update();
                    }

                    yield return null;
                }

                foreach (SubChapter subChapter in Data.SubChapters.Where(sc => sc.IsOptional && sc.Chapter.LifeCycle.Stage == Stage.Deactivating))
                {
                    subChapter.Chapter.LifeCycle.Abort();
                }

                while (Data.SubChapters.Any(sc => sc.Chapter.LifeCycle.Stage == Stage.Aborting))
                {
                    foreach (SubChapter sc in Data.SubChapters.Where(sc => sc.Chapter.LifeCycle.Stage == Stage.Aborting))
                    {
                        sc.Chapter.Update();
                    }

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
                foreach (SubChapter subChapter in Data.SubChapters)
                {
                    subChapter.Chapter.LifeCycle.MarkToFastForwardStage(Stage.Deactivating);
                }
            }
        }
    }
}