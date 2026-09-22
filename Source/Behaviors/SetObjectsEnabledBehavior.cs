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
    /// Sets enabled or disabled all specified objects.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/enable-objects-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class SetObjectsEnabledBehavior : Behavior<SetObjectsEnabledBehavior.EntityData>
    {
        /// <summary>
        /// Creates a set-objects-enabled behavior with default values.
        /// </summary>
        [JsonConstructor]
        public SetObjectsEnabledBehavior() : this(Guid.Empty, false)
        {
        }

        /// <summary>
        /// Creates a behavior that enables or disables the given objects.
        /// </summary>
        /// <param name="setEnabled">Whether the objects should be enabled (<c>true</c>) or disabled (<c>false</c>).</param>
        public SetObjectsEnabledBehavior(bool setEnabled) : this(Guid.Empty, setEnabled, false)
        {
        }

        /// <summary>
        /// Creates a behavior that enables or disables the object with the given unique id.
        /// </summary>
        /// <param name="objectId">The unique id of the object to enable or disable.</param>
        /// <param name="setEnabled">Whether the object should be enabled (<c>true</c>) or disabled (<c>false</c>).</param>
        /// <param name="revertOnDeactivate">If <c>true</c>, the enabled state is reverted when the step is deactivated.</param>
        public SetObjectsEnabledBehavior(Guid objectId, bool setEnabled, bool revertOnDeactivate = false)
        {
            Data.TargetObjects = new MultipleScenePropertyReference<IModifySceneObjectProperty>(objectId);
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
        /// Behavior data for <see cref="SetObjectsEnabledBehavior"/>.
        /// </summary>
        [DisplayName("Set Objects Enabled")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// The objects to enable or disable.
            /// </summary>
            [DataMember]
            [DisplayName("Objects")]
            public MultipleScenePropertyReference<IModifySceneObjectProperty> TargetObjects { get; set; }

            /// <summary>
            /// Whether the objects should be set active (<c>true</c>) or inactive (<c>false</c>).
            /// </summary>
            [DataMember]
            [HideInProcessInspector]
            public bool SetEnabled { get; set; }

            /// <summary>
            /// If <c>true</c>, the enabled state of the objects is reverted when the step is deactivated.
            /// </summary>
            [DataMember]
            [DisplayName("Revert after step is complete")]
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
                    return $"{setEnabled} {TargetObjects}";
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
                    property.SetActive(Data.SetEnabled);
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
                        property.SetActive(!Data.SetEnabled);
                }
            }
        }
    }
}