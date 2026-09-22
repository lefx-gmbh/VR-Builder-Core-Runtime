// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Properties;
using VRBuilder.Core.Runtime.Registry;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Conditions
{
    /// <summary>
    /// Condition which is completed when `TargetObject` gets inside `TriggerProperty`'s collider.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/objects-in-collider-condition.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class ObjectInColliderCondition : Condition<ObjectInColliderCondition.EntityData>
    {
        /// <summary>
        /// Creates an empty "object in collider" condition, used by the JSON deserializer.
        /// </summary>
        [JsonConstructor]
        public ObjectInColliderCondition() : this(Guid.Empty, Guid.Empty)
        {
        }

        /// <summary>
        /// Creates an "object in collider" condition for the given collider and set of objects.
        /// </summary>
        /// <param name="targetPosition">The trigger collider the objects have to enter.</param>
        /// <param name="multipleObjectsGuids">Unique ids of the scene objects that must enter the collider.</param>
        /// <param name="requiredTimeInTarget">How long the required object count must stay inside the collider, in seconds.</param>
        /// <param name="objectsRequiredInTrigger">Number of objects that must be inside the collider at the same time.</param>
        public ObjectInColliderCondition(IColliderWithTriggerProperty targetPosition, IReadOnlyList<Guid> multipleObjectsGuids, int requiredTimeInTarget, int objectsRequiredInTrigger)
            : this(ProcessReferenceUtils.GetUniqueIdFrom(targetPosition), multipleObjectsGuids, requiredTimeInTarget, objectsRequiredInTrigger)
        {
        }

        /// <summary>
        /// Creates an "object in collider" condition for the given collider and target object.
        /// </summary>
        /// <param name="targetPosition">The trigger collider the object has to enter.</param>
        /// <param name="targetObject">The scene object that must enter the collider.</param>
        /// <param name="requiredTimeInTarget">How long the required object count must stay inside the collider, in seconds.</param>
        /// <param name="objectsRequiredInTrigger">Number of objects that must be inside the collider at the same time.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public ObjectInColliderCondition(IColliderWithTriggerProperty targetPosition, ISceneObject targetObject, float requiredTimeInTarget = 0, float objectsRequiredInTrigger = 1)
            : this(ProcessReferenceUtils.GetUniqueIdFrom(targetPosition), ProcessReferenceUtils.GetUniqueIdFrom(targetObject), requiredTimeInTarget, objectsRequiredInTrigger)
        {
        }

        /// <summary>
        /// Creates an "object in collider" condition from the unique ids of the collider and target object.
        /// </summary>
        /// <param name="targetPosition">Unique id of the trigger collider the object has to enter.</param>
        /// <param name="targetObject">Unique id of the scene object that must enter the collider.</param>
        /// <param name="requiredTimeInTarget">How long the required object count must stay inside the collider, in seconds.</param>
        /// <param name="objectsRequiredInTrigger">Number of objects that must be inside the collider at the same time.</param>
        public ObjectInColliderCondition(Guid targetPosition, Guid targetObject, float requiredTimeInTarget = 0, float objectsRequiredInTrigger = 1)
        {
            Data.TriggerObject = new SingleScenePropertyReference<IColliderWithTriggerProperty>(targetPosition);
            Data.TargetObjects = new MultipleSceneObjectReference(targetObject);
            Data.RequiredTimeInside = requiredTimeInTarget;
            Data.ObjectsRequiredInTrigger = objectsRequiredInTrigger;
        }

        private ObjectInColliderCondition(Guid targetPosition, IReadOnlyList<Guid> targetObject, int requiredTimeInTarget, int objectsRequiredInTrigger)
        {
            Data.TriggerObject = new SingleScenePropertyReference<IColliderWithTriggerProperty>(targetPosition);
            Data.TargetObjects = new MultipleSceneObjectReference(targetObject);
            Data.RequiredTimeInside = requiredTimeInTarget;
            Data.ObjectsRequiredInTrigger = objectsRequiredInTrigger;
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
        /// The "object in collider" condition's data.
        /// </summary>
        [DisplayName("Move Object into Collider")]
        [DataContract(IsReference = true)]
        public class EntityData : IObjectInTargetData
        {
            /// <summary>
            /// The objects that have to enter the collider.
            /// </summary>
            [DataMember]
            [DisplayName("Objects")]
            [DisplayTooltip("Objects that must enter the collider.")]
            public MultipleSceneObjectReference TargetObjects { get; set; }

            /// <summary>
            /// The collider with trigger to enter.
            /// </summary>
            [DataMember]
            [DisplayName("Collider")]
            [DisplayTooltip("Trigger collider the objects must enter.")]
            public SingleScenePropertyReference<IColliderWithTriggerProperty> TriggerObject { get; set; }

            /// <summary>
            /// Number of objects that must be inside the collider at the same time for the condition to complete.
            /// </summary>
            [DataMember]
            [DisplayName("Required Object count")]
            [DisplayTooltip("Number of objects that must be inside the collider at the same time.")]
            public float ObjectsRequiredInTrigger { get; set; }

            /// <summary>
            /// True if the required objects have been inside the collider for the required time.
            /// </summary>
            public bool IsCompleted { get; set; }

            /// <inheritdoc />
            [HideInProcessInspector]
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    if (ObjectsRequiredInTrigger > 1 && TargetObjects.Values.Count() > 1)
                    {
                        return $"Move {ObjectsRequiredInTrigger} of {TargetObjects.Values.Count()} '{ServiceRegistry.Get<ISceneObjectRegistry>().SceneObjectGroups.GetLabel(TargetObjects.Guids.First())}' in collider {TriggerObject}";
                    }

                    return $"Move {TargetObjects} in collider {TriggerObject}";
                }
            }

            /// <inheritdoc />
            [DataMember]
            [DisplayName("Required seconds inside")]
            [DisplayTooltip("How long the required object count must stay inside the collider, in seconds.")]
            public float RequiredTimeInside { get; set; }

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
                if (Data.ObjectsRequiredInTrigger > Data.TargetObjects.Values.Count())
                {
                    ForwardingLogger.LogWarning($"The required object count {Data.ObjectsRequiredInTrigger} is bigger then the target objects count of {Data.TargetObjects.Values.Count()} and not completable.");
                }

                if (Data.ObjectsRequiredInTrigger < 1)
                {
                    ForwardingLogger.LogWarning($"The required object count {Data.ObjectsRequiredInTrigger} is below 1 and always completed.");
                }

                base.Start();
            }

            /// <inheritdoc />
            protected override bool IsInside()
            {
                int counter = 0;

                foreach (ISceneObject sceneObject in Data.TargetObjects.Values)
                {
                    counter += Data.TriggerObject.Value.IsTransformInsideTrigger(sceneObject) ? 1 : 0;
                }

                return counter >= Data.ObjectsRequiredInTrigger;
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
                if (Data.ObjectsRequiredInTrigger > 1 && Data.TargetObjects.Values.Any())
                {
                    int counter = 0;
                    foreach (var objs in Data.TargetObjects.Values)
                    {
                        Data.TriggerObject.Value.FastForwardEnter(objs);
                        counter++;
                        if (counter >= Data.ObjectsRequiredInTrigger)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    ISceneObject sceneObject = Data.TargetObjects.Values.FirstOrDefault();
                    Data.TriggerObject.Value.FastForwardEnter(sceneObject);
                }
            }
        }
    }
}