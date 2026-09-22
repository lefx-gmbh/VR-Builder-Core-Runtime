// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Conditions
{
    /// <summary>
    /// Condition that is completed when distance between `Target` and `TransformInRangeDetector` is closer than `range` units.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/object-nearby-condition.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class ObjectInRangeCondition : Condition<ObjectInRangeCondition.EntityData>
    {
        /// <summary>
        /// Creates an empty "object in range" condition, used by the JSON deserializer.
        /// </summary>
        [JsonConstructor]
        public ObjectInRangeCondition() : this(Guid.Empty, Guid.Empty, 0f)
        {
        }

        /// <summary>
        /// Creates an "object in range" condition for the given target and range detector.
        /// </summary>
        /// <param name="target">The tracked object whose distance to the reference is measured.</param>
        /// <param name="detector">The reference detector object used to measure distance from the tracked object.</param>
        /// <param name="range">Maximum distance in Unity units between the tracked and reference objects.</param>
        /// <param name="requiredTimeInTarget">How long the tracked object must stay within range, in seconds.</param>
        public ObjectInRangeCondition(ISceneObject target, ITransformInRangeDetectorProperty detector, float range, float requiredTimeInTarget = 0)
            : this(ProcessReferenceUtils.GetUniqueIdFrom(target), ProcessReferenceUtils.GetUniqueIdFrom(detector), range, requiredTimeInTarget)
        {
        }

        /// <summary>
        /// Creates an "object in range" condition from the unique ids of the target and range detector.
        /// </summary>
        /// <param name="targetId">Unique id of the tracked object whose distance is measured.</param>
        /// <param name="detector">Unique id of the reference detector object used to measure distance.</param>
        /// <param name="range">Maximum distance in Unity units between the tracked and reference objects.</param>
        /// <param name="requiredTimeInTarget">How long the tracked object must stay within range, in seconds.</param>
        public ObjectInRangeCondition(Guid targetId, Guid detector, float range, float requiredTimeInTarget = 0)
        {
            Data.TargetObject = new SingleSceneObjectReference(targetId);
            Data.ReferenceObject = new SingleScenePropertyReference<ITransformInRangeDetectorProperty>(detector);
            Data.Range = range;
            Data.RequiredTimeInside = requiredTimeInTarget;
        }

        /// <inheritdoc />
        public override IStageProcess GetActiveProcess()
        {
            return new ActiveProcess(Data);
        }

        /// <inheritdoc />
        protected override IAutocompleter GetAutocompleter()
        {
            return new EntityAutocompleter(Data);
        }

        /// <summary>
        /// The data of "object in range" condition.
        /// </summary>
        [DisplayName("Object Nearby")]
        public class EntityData : IObjectInTargetData
        {
            /// <summary>
            /// The tracked objects.
            /// </summary>
            [DataMember]
            [DisplayName("Tracked object")]
            [DisplayTooltip("The object whose distance to the reference is measured.")]
            public SingleSceneObjectReference TargetObject { get; set; }

            /// <summary>
            /// The object to measure distance from.
            /// </summary>
            [DataMember]
            [DisplayName("Reference object")]
            [DisplayTooltip("Reference point used to measure distance from the tracked object.")]
            public SingleScenePropertyReference<ITransformInRangeDetectorProperty> ReferenceObject { get; set; }

            /// <summary>
            /// The required distance between two objects to trigger the condition.
            /// </summary>
            [DataMember]
            [DisplayName("Range")]
            [DisplayTooltip("Maximum distance in Unity units between the tracked and reference objects.")]
            public float Range { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            [HideInProcessInspector]
            public string Name => $"Move {TargetObject} within {Range} units of {ReferenceObject}";

            /// <inheritdoc />
            [DataMember]
            [DisplayName("Required seconds inside")]
            [DisplayTooltip("How long the tracked object must stay within range, in seconds.")]
            public float RequiredTimeInside { get; set; }

            /// <summary>
            /// True if the tracked object has been within range of the reference object for the required time.
            /// </summary>
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }
        }

        private class ActiveProcess : ObjectInTargetActiveProcess<EntityData>
        {
            public ActiveProcess(EntityData data) : base(data)
            {
            }

            public override void Start()
            {
                Data.ReferenceObject.Value.SetTrackedTransform(Data.TargetObject.Value);
                Data.ReferenceObject.Value.DetectionRange = Data.Range;

                base.Start();
            }

            /// <inheritdoc />
            protected override bool IsInside()
            {
                return Data.ReferenceObject.Value.IsTargetInsideRange();
            }
        }

        private class EntityAutocompleter : Autocompleter<EntityData>
        {
            public EntityAutocompleter(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Complete()
            {
                Data.ReferenceObject.Value.ForceMoveToTracked();
            }
        }
    }
}