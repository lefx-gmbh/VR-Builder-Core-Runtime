// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.EntityOwners;
using VRBuilder.Core.EntityOwners.ParallelEntityCollection;
using VRBuilder.Core.Exceptions;
using VRBuilder.Core.Runtime.Registry;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core
{
    /// <summary>
    /// A chapter of a process <see cref="Process"/>.
    /// </summary>
    [DataContract(IsReference = true)]
    public class Chapter : Entity<Chapter.EntityData>, IChapter
    {
        /// <summary>
        /// Creates a chapter with no name and no first step.
        /// </summary>
        protected Chapter() : this(null, null)
        {
        }

        /// <summary>
        /// Creates a chapter with the given name and first step.
        /// </summary>
        /// <param name="name">The name of the chapter.</param>
        /// <param name="firstStep">The first step of the chapter, or <c>null</c>.</param>
        public Chapter(string name, IStep firstStep)
        {
            ChapterMetadata = new ChapterMetadata();
            ChapterMetadata.Guid = Id;

            Data.Name = name;
            Data.FirstStep = firstStep;
            Data.Steps = new List<IStep>();

            if (firstStep != null)
            {
                Data.Steps.Add(firstStep);
            }

            if (ServiceRegistry.Get<IRuntimeService>().LifeCycleLogging.LogChapters)
            {
                LifeCycle.StageChanged += (sender, args) => { ForwardingLogger.LogFormat("<b>Chapter</b> <i>'{0}'</i> is <b>{1}</b>.\n", Data.Name, LifeCycle.Stage.ToString()); };
            }
        }

        /// <inheritdoc />
        [DataMember]
        public ChapterMetadata ChapterMetadata { get; set; }

        /// <inheritdoc />
        public override void RegenerateId()
        {
            base.RegenerateId();

            if (ChapterMetadata != null)
            {
                ChapterMetadata.Guid = Id;
            }
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new StopEntityIteratingProcess<IStep>(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetAbortingProcess()
        {
            return new ParallelAbortingProcess<EntityData>(Data);
        }

        /// <inheritdoc />
        IChapterData IDataOwner<IChapterData>.Data
        {
            get { return Data; }
        }

        /// <inheritdoc />
        protected override IConfigurator GetConfigurator()
        {
            return new SequenceConfigurator<IStep>(Data);
        }

        /// <summary>
        /// Creates a new <see cref="IChapter"/>.
        /// </summary>
        /// <param name="name"><see cref="IChapter"/>'s name.</param>
        public static IChapter Create(string name)
        {
            return new Chapter(name, null);
        }

        /// <summary>
        /// The chapter's data class.
        /// </summary>
        [DataContract(IsReference = true)]
        public class EntityData : EntityCollectionData<IStep>, IChapterData
        {
            /// <inheritdoc />
            [DataMember]
            [HideInProcessInspector]
            public string Name { get; set; }

            /// <summary>
            /// The first step of the chapter.
            /// </summary>
            [DataMember]
            public IStep FirstStep { get; set; }

            /// <summary>
            /// All steps of the chapter.
            /// </summary>
            [DataMember]
            public IList<IStep> Steps { get; set; }

            /// <inheritdoc />
            public override IEnumerable<IStep> GetChildren()
            {
                return Steps.ToArray();
            }

            /// <inheritdoc />
            public void SetName(string name)
            {
                Name = name;
            }

            /// <inheritdoc />
            public IMode Mode { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public IStep Current { get; set; }

            /// <inheritdoc />
            IEntity IEntitySequenceData.Current => Current;
        }

        private class ActivatingProcess : EntityIteratingProcess<IEntitySequenceDataWithMode<IStep>, IStep>
        {
            private readonly IStep firstStep;

            private IEnumerator<IStep> enumerator;

            public ActivatingProcess(IChapterData data) : base(data)
            {
                firstStep = data.FirstStep;
            }

            private IEnumerator<IStep> GetChildren()
            {
                IStep current = firstStep;

                while (current != null)
                {
                    yield return current;

                    current = current.Data.Transitions.Data.Transitions.First(transition => transition.IsCompleted).Data.TargetStepReference.Entity;
                }
            }

            /// <inheritdoc />
            public override void Start()
            {
                enumerator = GetChildren();
                base.Start();
            }

            /// <inheritdoc />
            protected override bool ShouldActivateCurrent()
            {
                return true;
            }

            /// <inheritdoc />
            protected override bool ShouldDeactivateCurrent()
            {
                return Data.Current.Data.Transitions.Data.Transitions.Any(transition => transition.IsCompleted);
            }

            /// <inheritdoc />
            public override void End()
            {
                enumerator = null;
                base.End();
            }

            /// <inheritdoc />
            protected override bool TryNext(out IStep entity)
            {
                if (enumerator != null && enumerator.MoveNext())
                {
                    entity = enumerator.Current;
                    return true;
                }
                else
                {
                    entity = null;
                    return false;
                }
            }

            /// <inheritdoc />
            public override void FastForward()
            {
                if (Data.Current == null)
                {
                    return;
                }

                if (Data.Current.FindPathInGraph(step => step.Data.Transitions.Data.Transitions.Select(transition => transition.Data.TargetStepReference.Entity), null, out IList<IStep> pathToChapterEnd) == false)
                {
                    throw new InvalidStateException("The end of the chapter is not reachable from the current step.");
                }

                foreach (IStep step in pathToChapterEnd)
                {
                    if (Data.Current.LifeCycle.Stage == Stage.Inactive)
                    {
                        Data.Current.LifeCycle.Activate();
                    }

                    Data.Current.LifeCycle.MarkToFastForward();

                    ITransition toAutocomplete = Data.Current.Data.Transitions.Data.Transitions.First(transition => transition.Data.TargetStepReference.Entity == step);
                    if (toAutocomplete.IsCompleted == false)
                    {
                        toAutocomplete.Autocomplete();
                    }

                    Data.Current.LifeCycle.Deactivate();

                    Data.Current = step;
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            ChapterMetadata = ChapterMetadata ?? new ChapterMetadata();

            if (ChapterMetadata.Guid == Guid.Empty)
            {
                ChapterMetadata.Guid = Id;
            }
            else
            {
                SetId(ChapterMetadata.Guid);
            }
        }
    }
}