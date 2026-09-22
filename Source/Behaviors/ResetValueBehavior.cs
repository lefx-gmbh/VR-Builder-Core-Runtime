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
    /// A behavior that reset a data property to its default value specified in the inspector.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://www.mindport.co/vr-builder-tutorials/states-data-add-on")]
    public class ResetValueBehavior : Behavior<ResetValueBehavior.EntityData>
    {
        /// <summary>
        /// Creates a new <see cref="ResetValueBehavior"/>; the target property must be configured later.
        /// </summary>
        [JsonConstructor]
        public ResetValueBehavior() : this(Guid.Empty)
        {
        }

        /// <summary>
        /// Creates a behavior that resets the data property identified by <paramref name="propertyId"/>.
        /// </summary>
        /// <param name="propertyId">Unique id of the data property to reset.</param>
        public ResetValueBehavior(Guid propertyId)
        {
            Data.Properties = new MultipleScenePropertyReference<IDataPropertyBase>(propertyId);
        }

        /// <summary>
        /// Creates a behavior that resets <paramref name="property"/> to its default value.
        /// </summary>
        /// <param name="property">Data property to reset.</param>
        public ResetValueBehavior(IDataPropertyBase property) : this(ProcessReferenceUtils.GetUniqueIdFrom(property))
        {
        }

        /// <inheritdoc />
        public override IStageProcess GetActivatingProcess()
        {
            return new ActivatingProcess(Data);
        }

        /// <summary>
        /// The <see cref="ResetValueBehavior"/> behavior data.
        /// </summary>
        [DisplayName("Reset Value")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData
        {
            /// <summary>
            /// Data properties that are reset to their default values on activation.
            /// </summary>
            [DataMember]
            [DisplayName("Data Properties")]
            public MultipleScenePropertyReference<IDataPropertyBase> Properties;

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name => $"Reset {Properties} to default";
        }

        private class ActivatingProcess : StageProcess<EntityData>
        {
            public ActivatingProcess(EntityData data) : base(data)
            {
            }

            /// <inheritdoc />
            public override void Start()
            {
                foreach (IDataPropertyBase dataProperty in Data.Properties.Values)
                {
                    dataProperty.ResetValue();
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
    }
}