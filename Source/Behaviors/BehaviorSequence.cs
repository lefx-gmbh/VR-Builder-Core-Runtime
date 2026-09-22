using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.EntityOwners;
using VRBuilder.Core.EntityOwners.ParallelEntityCollection;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// A collection of behaviors that are activated and deactivated after each other.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/behavior-sequence-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class BehaviorSequence : Behavior<BehaviorSequence.EntityData>
    {
        /// <summary>
        /// Creates an empty behavior sequence, used by the JSON deserializer.
        /// </summary>
        [JsonConstructor]
        public BehaviorSequence() : this(default(bool), new List<IBehavior>())
        {
        }

        /// <summary>
        /// Creates a blocking behavior sequence with the given <paramref name="behaviors"/>.
        /// </summary>
        /// <param name="playsOnRepeat">If <c>true</c>, the sequence loops continuously while the step is active.</param>
        /// <param name="behaviors">Child behaviors activated and deactivated one after another.</param>
        public BehaviorSequence(bool playsOnRepeat, IList<IBehavior> behaviors)
        {
            Data.PlaysOnRepeat = playsOnRepeat;
            Data.Behaviors = new List<IBehavior>(behaviors);
            Data.IsBlocking = true;
        }

        /// <summary>
        /// Creates a behavior sequence with the given <paramref name="behaviors"/> and blocking behavior.
        /// </summary>
        /// <param name="playsOnRepeat">If <c>true</c>, the sequence loops continuously while the step is active.</param>
        /// <param name="behaviors">Child behaviors activated and deactivated one after another.</param>
        /// <param name="isBlocking">If <c>true</c>, the sequence prevents step completion while it is running.</param>
        public BehaviorSequence(bool playsOnRepeat, IList<IBehavior> behaviors, bool isBlocking) : this(playsOnRepeat, behaviors)
        {
            Data.IsBlocking = isBlocking;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new IteratingProcess(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetDeactivatingProcess()
        {
            return new StopEntityIteratingProcess<IBehavior>(Data);
        }

        /// <inheritdoc />
        public override IStageProcess GetAbortingProcess()
        {
            return new ParallelAbortingProcess<EntityData>(Data);
        }

        /// <inheritdoc />
        protected override IConfigurator GetConfigurator()
        {
            return new SequenceConfigurator<IBehavior>(Data);
        }

        /// <summary>
        /// Behavior sequence's data.
        /// </summary>
        [DisplayName("Behavior Sequence")]
        [DataContract(IsReference = true)]
        public class EntityData : EntityCollectionData<IBehavior>, IEntitySequenceDataWithMode<IBehavior>, IBackgroundBehaviorData
        {
            /// <summary>
            /// Are child behaviors activated only once or the collection is cycled.
            /// </summary>
            [DisplayName("Repeat")]
            [DataMember]
            public bool PlaysOnRepeat { get; set; }

            /// <summary>
            /// List of child behaviors.
            /// </summary>
            [DataMember]
            [DisplayName("Child behaviors")]
            [Foldable, ReorderableListOf(typeof(FoldableAttribute), typeof(HelpAttribute), typeof(MenuAttribute)), ExtendableList]
            public List<IBehavior> Behaviors { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string behaviors = "";

                    if (Behaviors.Count == 0)
                    {
                        behaviors = "no behavior";
                    }
                    else
                    {
                        foreach (IBehavior behavior in Behaviors)
                        {
                            behaviors += behavior.Data.Name;
                            if (behavior != Behaviors.Last())
                            {
                                behaviors += ", ";
                            }
                        }
                    }

                    return $"Sequence ({behaviors})";
                }
            }

            /// <inheritdoc />
            public bool IsBlocking { get; set; }

            /// <inheritdoc />
            public override IEnumerable<IBehavior> GetChildren()
            {
                return Behaviors.ToList();
            }

            /// <inheritdoc />
            public IMode Mode { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public IBehavior Current { get; set; }

            /// <inheritdoc />
            IEntity IEntitySequenceData.Current => Current;
        }

        private class IteratingProcess : EntityIteratingProcess<IEntitySequenceDataWithMode<IBehavior>, IBehavior>
        {
            private IEnumerator<IBehavior> enumerator;


            public IteratingProcess(IEntitySequenceDataWithMode<IBehavior> data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                base.Start();
                enumerator = Data.GetChildren().GetEnumerator();
            }

            /// <inheritdoc />
            protected override bool ShouldActivateCurrent()
            {
                return true;
            }

            /// <inheritdoc />
            protected override bool ShouldDeactivateCurrent()
            {
                return true;
            }

            /// <inheritdoc />
            protected override bool TryNext(out IBehavior entity)
            {
                if (enumerator == null || enumerator.MoveNext() == false)
                {
                    entity = default(IBehavior);
                    return false;
                }
                else
                {
                    entity = enumerator.Current;
                    return true;
                }
            }
        }

        private class ActiveProcess : StageProcess<EntityData>
        {
            private readonly IStageProcess childProcess;

            public ActiveProcess(EntityData data) : base(data)
            {
                childProcess = new IteratingProcess(Data);
            }

            /// <inheritdoc />
            public override void Start()
            {
                childProcess.Start();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                if (Data.PlaysOnRepeat == false)
                {
                    yield break;
                }

                int endlessLoopCheck = 0;

                while (endlessLoopCheck < 100000)
                {
                    IEnumerator update = childProcess.Update();

                    while (update.MoveNext())
                    {
                        yield return null;
                    }

                    childProcess.End();

                    childProcess.Start();

                    endlessLoopCheck++;
                }
            }

            /// <inheritdoc />
            public override void End()
            {
            }

            /// <inheritdoc />
            public override void FastForward()
            {
                childProcess.FastForward();
                childProcess.End();
            }
        }
    }
}