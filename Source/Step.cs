// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.EntityOwners;
using VRBuilder.Core.EntityOwners.FoldedEntityCollection;
using VRBuilder.Core.EntityOwners.ParallelEntityCollection;
using VRBuilder.Core.Primitives;
using VRBuilder.Core.Properties;
using VRBuilder.Core.RestrictiveEnvironment;
using VRBuilder.Core.Runtime.Registry;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.StepLocking;
using VRBuilder.Utils;

namespace VRBuilder.Core
{
    /// <summary>
    /// An implementation of <see cref="IStep"/> interface.
    /// </summary>
    [DataContract(IsReference = true)]
    public class Step : Entity<Step.EntityData>, IStep
    {
        /// <summary>
        /// Creates a new step with no name.
        /// </summary>
        protected Step() : this(null)
        {
        }

        /// <summary>
        /// Creates a new step with the given name.
        /// </summary>
        /// <param name="name">The name of the step.</param>
        public Step(string name)
        {
            StepMetadata = new StepMetadata();
            StepMetadata.Guid = Id;

            Data.Transitions = new TransitionCollection();
            Data.Behaviors = new BehaviorCollection();
            Data.Name = name;

            if (ServiceRegistry.Get<IRuntimeService>()?.LifeCycleLogging.LogSteps == true)
            {
                LifeCycle.StageChanged += (sender, args) => { ForwardingLogger.LogFormat("{0}<b>Step</b> <i>'{1}'</i> is <b>{2}</b>.\n", ConsoleUtils.GetTabs(), Data.Name, LifeCycle.Stage); };
            }
        }

        ///<inheritdoc />
        [DataMember]
        public StepMetadata StepMetadata { get; set; }

        /// <inheritdoc />
        public override void RegenerateId()
        {
            base.RegenerateId();

            if (StepMetadata != null)
            {
                StepMetadata.Guid = Id;
            }
        }

        ///<inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new CompositeProcess(new FoldedActivatingProcess<IStepChild>(Data));
        }

        ///<inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new CompositeProcess(new FoldedActiveProcess<IStepChild>(Data), new ActiveProcess(Data), new UnlockProcess(Data));
        }

        ///<inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new CompositeProcess(new FoldedDeactivatingProcess<IStepChild>(Data), new LockProcess(Data));
        }

        ///<inheritdoc />
        public override IStageProcess GetAbortingProcess()
        {
            return new CompositeProcess(new AbortingProcess(Data), new ParallelAbortingProcess<EntityData>(Data));
        }

        ///<inheritdoc />
        
        ///<inheritdoc />
        IStepData IDataOwner<IStepData>.Data
        {
            get { return Data; }
        }

        public override void Configure(IMode mode)
        {
#if UNITY_EDITOR
            try
            {
#endif
            base.Configure(mode);
#if UNITY_EDITOR
            }
            catch (Exception e)
            {
                string fullPath = EntityPathUtils.BuildRichTextEntityPath(this);
                ForwardingLogger.LogError($"Configure failed at {fullPath}\nException: {e.Message}");
                ForwardingLogger.LogException(e);
            }
#endif
        }

        ///<inheritdoc />
        protected override IConfigurator GetConfigurator()
        {
            return new FoldedLifeCycleConfigurator<IStepChild>(Data);
        }

        /// <summary>
        /// Creates a new <see cref="IStep"/>.
        /// </summary>
        /// <param name="name"><see cref="IStep"/>'s name.</param>
        /// <param name="position">The step's position in the process graph.</param>
        /// <param name="stepType">The step's type identifier.</param>
        /// <returns>The created <see cref="IStep"/>.</returns>
        public static IStep Create(string name, IVector2 position = default, string stepType = "default")
        {
            IStep step = new Step(name);
            step.StepMetadata.Position = position;
            step.StepMetadata.StepType = stepType;
            // PostProcessEntity<IStep>(step);

            return step;
        }

        /// <summary>
        /// The data of a <see cref="Step"/>.
        /// </summary>
        public class EntityData : EntityCollectionData<IStepChild>, IStepData, ILockableStepData
        {
            /// <summary>
            /// Creates an empty step data.
            /// </summary>
            public EntityData()
            {
            }

            ///<inheritdoc />
            [DataMember]
            [HideInProcessInspector]
            public IEnumerable<LockablePropertyReference> ToUnlock { get; set; } = new List<LockablePropertyReference>();

            /// <summary>
            /// The groups to unlock when the step completes.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            public IDictionary<Guid, IEnumerable<Type>> GroupsToUnlock { get; set; } = new Dictionary<Guid, IEnumerable<Type>>();

            ///<inheritdoc />
            [DataMember]
            [DrawingPriority(0)]
            [HideInProcessInspector]
            public string Name { get; set; }

            ///<inheritdoc />
            [DataMember]
            [DrawingPriority(1)]
            [UsesSpecificProcessDrawer("MultiLineStringDrawer")]
            public string Description { get; set; }

            ///<inheritdoc />
            [DataMember]
            [HideInProcessInspector]
            public IBehaviorCollection Behaviors { get; set; }

            ///<inheritdoc />
            [DataMember]
            [HideInProcessInspector]
            public ITransitionCollection Transitions { get; set; }

            ///<inheritdoc />
            public override IEnumerable<IStepChild> GetChildren()
            {
                return new List<IStepChild>
                {
                    Behaviors,
                    Transitions
                };
            }

            /// <inheritdoc />
            public void SetName(string name)
            {
                Name = name;
            }

            ///<inheritdoc />
            [IgnoreDataMember]
            public IStepChild Current { get; set; }

            /// <inheritdoc />
            IEntity IEntitySequenceData.Current => Current;

            public IMode Mode { get; set; }
        }

        private class UnlockProcess : StageProcess<EntityData>
        {
            private readonly IEnumerable<LockablePropertyData> toUnlock;

            public UnlockProcess(EntityData data) : base(data)
            {
                toUnlock = Data.ToUnlock.Select(reference => new LockablePropertyData(reference.GetProperty())).ToList();

                foreach (Guid tag in Data.GroupsToUnlock.Keys)
                {
                    foreach (ISceneObject sceneObject in ServiceRegistry.Get<ISceneObjectRegistry>().GetObjects(tag))
                    {
                        toUnlock = toUnlock.Union(sceneObject.Properties.Where(property => Data.GroupsToUnlock[tag].Contains(property.GetType())).Select(property => new LockablePropertyData(property as ILockableProperty))).ToList();
                    }
                }
            }

            ///<inheritdoc />
            public override void Start()
            {
                ServiceRegistry.Get<IStepLockService>()?.Unlock(Data, toUnlock);
            }

            ///<inheritdoc />
            public override IEnumerator Update()
            {
                yield return null;
            }

            ///<inheritdoc />
            public override void End()
            {
            }

            ///<inheritdoc />
            public override void FastForward()
            {
            }
        }

        private class LockProcess : StageProcess<EntityData>
        {
            private readonly IEnumerable<LockablePropertyData> toUnlock;

            public LockProcess(EntityData data) : base(data)
            {
                toUnlock = Data.ToUnlock.Select(reference => new LockablePropertyData(reference.GetProperty())).ToList();

                foreach (Guid tag in Data.GroupsToUnlock.Keys)
                {
                    foreach (ISceneObject sceneObject in ServiceRegistry.Get<ISceneObjectRegistry>().GetObjects(tag))
                    {
                        toUnlock = toUnlock.Union(sceneObject.Properties.Where(property => Data.GroupsToUnlock[tag].Contains(property.GetType())).Select(property => new LockablePropertyData(property as ILockableProperty))).ToList();
                    }
                }
            }

            ///<inheritdoc />
            public override void Start()
            {
            }

            ///<inheritdoc />
            public override IEnumerator Update()
            {
                yield return null;
            }

            ///<inheritdoc />
            public override void End()
            {
                ServiceRegistry.Get<IStepLockService>()?.Lock(Data, toUnlock);
            }

            ///<inheritdoc />
            public override void FastForward()
            {
            }
        }

        private class ActiveProcess : StageProcess<EntityData>
        {
            private readonly IEnumerable<LockablePropertyData> toUnlock;

            public ActiveProcess(EntityData data) : base(data)
            {
            }

            ///<inheritdoc />
            public override void Start()
            {
            }

            ///<inheritdoc />
            public override IEnumerator Update()
            {
                while (Data.Transitions.Data.Transitions.Any(transition => transition.IsCompleted) == false)
                {
                    yield return null;
                }
            }

            ///<inheritdoc />
            public override void End()
            {
            }

            ///<inheritdoc />
            public override void FastForward()
            {
            }
        }

        private class AbortingProcess : InstantProcess<EntityData>
        {
            private readonly IEnumerable<LockablePropertyData> lockableProperties;

            public AbortingProcess(EntityData data) : base(data)
            {
                lockableProperties = Data.ToUnlock.Select(reference => new LockablePropertyData(reference.GetProperty())).ToList();

                foreach (Guid tag in Data.GroupsToUnlock.Keys)
                {
                    foreach (ISceneObject sceneObject in ServiceRegistry.Get<ISceneObjectRegistry>().GetObjects(tag))
                    {
                        lockableProperties = lockableProperties.Union(sceneObject.Properties.Where(property => Data.GroupsToUnlock[tag].Contains(property.GetType())).Select(property => new LockablePropertyData(property as ILockableProperty))).ToList();
                    }
                }
            }

            public override void Start()
            {
                ServiceRegistry.Get<IStepLockService>()?.Lock(Data, lockableProperties);
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            StepMetadata = StepMetadata ?? new StepMetadata();

            if (StepMetadata.Guid == Guid.Empty)
            {
                StepMetadata.Guid = Id;
            }
            else
            {
                SetId(StepMetadata.Guid);
            }
        }
    }
}