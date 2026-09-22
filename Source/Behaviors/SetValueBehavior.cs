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
    /// A behavior that sets a data property to a specified value.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/states-data-add-on")]
    public class SetValueBehavior<T> : Behavior<SetValueBehavior<T>.EntityData>
    {
        /// <summary>
        /// Creates a new <see cref="SetValueBehavior{T}"/> without a target property and value.
        /// </summary>
        [JsonConstructor]
        public SetValueBehavior() : this(Guid.Empty, default)
        {
        }

        /// <summary>
        /// Creates a behavior that sets the data property identified by <paramref name="propertyId"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="propertyId">Unique id of the scene data property to set.</param>
        /// <param name="value">Value assigned to the property on activation.</param>
        public SetValueBehavior(Guid propertyId, T value)
        {
            Data.DataProperties = new MultipleScenePropertyReference<IDataProperty<T>>(propertyId);
            Data.NewValue = value;
        }

        /// <summary>
        /// Creates a behavior that sets <paramref name="property"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="property">Scene data property whose value is set on activation.</param>
        /// <param name="value">Value assigned to the property on activation.</param>
        public SetValueBehavior(IDataProperty<T> property, T value) : this(ProcessReferenceUtils.GetUniqueIdFrom(property), value)
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <summary>
        /// The <see cref="SetValueBehavior{T}"/> behavior data.
        /// </summary>
        [DisplayName("Set Value")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Scene data properties whose value is set to <see cref="NewValue"/> on activation.
            /// </summary>
            [DataMember]
            [DisplayName("Data Property")]
            public MultipleScenePropertyReference<IDataProperty<T>> DataProperties { get; set; }

            /// <summary>
            /// Value assigned to each <see cref="DataProperties"/> on activation.
            /// </summary>
            [DataMember]
            [DisplayName("Value")]
            public T NewValue { get; set; }

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name => $"Set {DataProperties} to {NewValue}";
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                foreach (IDataProperty<T> dataProperty in Data.DataProperties.Values)
                {
                    dataProperty.SetValue(Data.NewValue);
                }
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

        /// <summary>
        /// Constructs concrete types in order for them to be seen by IL2CPP's ahead of time compilation.
        /// </summary>
        private class AOTHelper
        {
            SetValueBehavior<bool> bln = new SetValueBehavior<bool>();
            SetValueBehavior<float> flt = new SetValueBehavior<float>();
            SetValueBehavior<string> str = new SetValueBehavior<string>();
        }
    }
}