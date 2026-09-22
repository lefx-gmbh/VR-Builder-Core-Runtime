// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Enables/disables all components of a given type on a given game object.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/enable-components-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class SetComponentEnabledBehavior : Behavior<SetComponentEnabledBehavior.EntityData>
    {
        /// <summary>
        /// Creates a new <see cref="SetComponentEnabledBehavior"/>; the target and component type must be configured later.
        /// </summary>
        [JsonConstructor]
        public SetComponentEnabledBehavior() : this(Guid.Empty, "", false, false)
        {
        }

        /// <summary>
        /// Creates a behavior that enables or disables components on the target objects.
        /// </summary>
        /// <param name="setEnabled">If <c>true</c>, components are enabled; otherwise they are disabled.</param>
        /// <param name="name">Display name of the behavior.</param>
        public SetComponentEnabledBehavior(bool setEnabled, string name = "Set Component Enabled") : this(Guid.Empty, "", setEnabled, false)
        {
        }

        /// <summary>
        /// Creates a behavior that enables or disables components of type <paramref name="componentType"/> on the scene object identified by <paramref name="objectId"/>.
        /// </summary>
        /// <param name="objectId">Unique id of the scene object whose components are affected.</param>
        /// <param name="componentType">Type name of the components to enable or disable.</param>
        /// <param name="setEnabled">If <c>true</c>, components are enabled; otherwise they are disabled.</param>
        /// <param name="revertOnDeactivate">If <c>true</c>, the component state reverts to its original state on deactivation.</param>
        public SetComponentEnabledBehavior(Guid objectId, string componentType, bool setEnabled, bool revertOnDeactivate)
        {
            Data.TargetObjects = new MultipleScenePropertyReference<IModifySceneComponentProperty>(objectId);
            Data.ComponentType = componentType;
            Data.SetEnabled = setEnabled;
            Data.RevertOnDeactivation = revertOnDeactivate;
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

        /// <summary>
        /// The behavior's data.
        /// </summary>
        [DisplayName("Set Component Enabled")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Object the target component is on.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            public MultipleScenePropertyReference<IModifySceneComponentProperty> TargetObjects { get; set; }

            /// <summary>
            /// Type of components to interact with.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            public string ComponentType { get; set; }

            /// <summary>
            /// If true, the component will be enabled, otherwise it will disabled.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            public bool SetEnabled { get; set; }

            /// <summary>
            /// If true, the component will revert to its original state when the behavior deactivates.
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            public bool RevertOnDeactivation { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name
            {
                get
                {
                    string setEnabled = SetEnabled ? "Enable" : "Disable";
                    string componentType = string.IsNullOrEmpty(ComponentType) ? "<none>" : ComponentType;
                    return $"{setEnabled} {componentType} for {TargetObjects}";
                }
            }
        }

        private class ActivatingProcess : InstantProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                foreach (var property in Data.TargetObjects.Values)
                    property.SetComponentActive(Data.ComponentType, Data.SetEnabled);
            }
        }

        private class DeactivatingProcess : InstantProcess<EntityData>
        {
            public DeactivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                if (Data.RevertOnDeactivation)
                {
                    foreach (var property in Data.TargetObjects.Values)
                        property.SetComponentActive(Data.ComponentType, Data.SetEnabled);
                }
            }
        }
    }
}