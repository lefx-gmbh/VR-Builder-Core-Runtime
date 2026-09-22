// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// This behavior changes the parent of a game object in the scene hierarchy. It can accept a null parent, in which case the object will be unparented.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/set-parent-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class SetParentBehavior : Behavior<SetParentBehavior.EntityData>
    {
        /// <summary>
        /// Creates a set-parent behavior with empty targets.
        /// </summary>
        [JsonConstructor]
        public SetParentBehavior() : this(Guid.Empty, Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a set-parent behavior that reparents the target object to the parent object.
        /// </summary>
        /// <param name="target">The object to reparent.</param>
        /// <param name="parent">The new parent object, or <c>null</c> to unparent the target.</param>
        /// <param name="snapToParentTransform">If <c>true</c>, the object is moved to the parent's transform.</param>
        public SetParentBehavior(ISceneObject target, ISceneObject parent, bool snapToParentTransform = false) : this(ProcessReferenceUtils.GetUniqueIdFrom(target), ProcessReferenceUtils.GetUniqueIdFrom(parent), snapToParentTransform)
        {
        }

        /// <summary>
        /// Creates a set-parent behavior from the unique ids of the target and parent objects.
        /// </summary>
        /// <param name="target">The unique id of the object to reparent.</param>
        /// <param name="parent">The unique id of the new parent object, or <see cref="Guid.Empty"/> to unparent the target.</param>
        /// <param name="snapToParentTransform">If <c>true</c>, the object is moved to the parent's transform.</param>
        public SetParentBehavior(Guid target, Guid parent, bool snapToParentTransform = false)
        {
            Data.TargetObject = new SingleScenePropertyReference<IModifyParentProperty>(target);
            Data.ParentObject = new SingleSceneObjectReference(parent);
            Data.SnapToParentTransform = snapToParentTransform;
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <summary>
        /// The data for a <see cref="SetParentBehavior"/>.
        /// </summary>
        [DisplayName("Set Parent")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Process object to reparent.
            /// </summary>
            [DataMember]
            [DisplayName("Target Object")]
            [DisplayTooltip("Process object to reparent.")]
            public SingleScenePropertyReference<IModifyParentProperty> TargetObject { get; set; }

            /// <summary>
            /// New parent game object.
            /// </summary>
            [DataMember]
            [DisplayName("Parent Object")]
            [DisplayTooltip("New parent game object. Leave empty to unparent the target object.")]
            public SingleSceneObjectReference ParentObject { get; set; }

            /// <summary>
            /// If true, the object will be moved to the parent's transform.
            /// </summary>
            [DataMember]
            [DisplayName("Snap to parent transform")]
            public bool SnapToParentTransform { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name => ParentObject.HasValue() ? $"Make {TargetObject} child of {ParentObject}" : $"Unparent {TargetObject}";
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                if (Data.ParentObject.HasValue())
                    Data.TargetObject.Value.SetParent(Data.ParentObject.Value, Data.SnapToParentTransform);
                else
                    Data.TargetObject.Value.UnsetParent();
            }

            /// <inheritdoc />
            public override IEnumerator Update()
            {
                yield return null;
            }

            /// <inheritdoc />
            public override void End()
            {
            }

            /// <inheritdoc />
            public override void FastForward()
            {
            }
        }
    }
}