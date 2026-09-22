// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Configuration.Modes;
using VRBuilder.Core.Primitives;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Behavior that highlights the target <see cref="ISceneObject"/> with the specified color until the behavior is being deactivated.
    /// </summary>
    [DataContract(IsReference = true)]
    [HelpLink("https://mindport-gmbh.github.io/VR-Builder-Documentation/articles/core/highlight-object-behavior.html?utm_source=unity_editor&utm_medium=referral&utm_campaign=from_unity&utm_id=from_unity")]
    public class HighlightObjectBehavior : ColorHighlightBehaviorBase<HighlightObjectBehavior.EntityData, IHighlightProperty>, IObjectHighlightBehavior
    {
        private static readonly ColorData defaultHighlightColor = new ColorData(231, 64, 255, 126);

        /// <summary>
        /// Creates a new <see cref="HighlightObjectBehavior"/>; the target object must be configured later.
        /// </summary>
        [JsonConstructor]
        public HighlightObjectBehavior() : this(Guid.Empty, defaultHighlightColor)
        {
        }

        /// <summary>
        /// Creates a behavior that highlights the scene object identified by <paramref name="objectId"/> with <paramref name="highlightColor"/>.
        /// </summary>
        /// <param name="objectId">Unique id of the scene object to highlight.</param>
        /// <param name="highlightColor">Color used for the highlight.</param>
        public HighlightObjectBehavior(Guid objectId, IColor highlightColor) : base(objectId, highlightColor)
        {
        }

        /// <summary>
        /// Creates a behavior that highlights <paramref name="target"/> with the default highlight color.
        /// </summary>
        /// <param name="target">Scene object property to highlight.</param>
        public HighlightObjectBehavior(IHighlightProperty target) : this(target, defaultHighlightColor)
        {
        }

        /// <summary>
        /// Creates a behavior that highlights <paramref name="target"/> with <paramref name="highlightColor"/>.
        /// </summary>
        /// <param name="target">Scene object property to highlight.</param>
        /// <param name="highlightColor">Color used for the highlight.</param>
        public HighlightObjectBehavior(IHighlightProperty target, IColor highlightColor) : this(ProcessReferenceUtils.GetUniqueIdFrom(target), highlightColor)
        {
        }

        /// <inheritdoc />
        protected override void ApplyHighlight(IHighlightProperty property, IColor color)
        {
            property?.Highlight(color);
        }

        /// <inheritdoc />
        protected override void RemoveHighlight(IHighlightProperty property)
        {
            property?.Unhighlight();
        }

        /// <summary>
        /// "Highlight object" behavior's data.
        /// </summary>
        [DisplayName("Highlight Object")]
        [DataContract(IsReference = true)]
        public class EntityData : IBehaviorData, IColorHighlightBehaviorData<IHighlightProperty>
        {
            private ModeParameter<IColor> customColor;

            /// <inheritdoc />
            public Metadata Metadata { get; set; }

            /// <inheritdoc />
            [IgnoreDataMember]
            public string Name => $"Highlight {TargetObjects}";

            /// <summary>
            /// <see cref="ModeParameter{T}"/> of the highlight color.
            /// Process modes can change the highlight color.
            /// </summary>
            public ModeParameter<IColor> CustomColor
            {
                get { return customColor ??= new ModeParameter<IColor>("HighlightColor", defaultHighlightColor); }
                set { customColor = value; }
            }

            /// <summary>
            /// Highlight color set in the Step Inspector.
            /// </summary>
            [DataMember(Name = "HighlightColor")]
            [JsonProperty("HighlightColor")]
            [DisplayName("Color")]
            public IColor Color
            {
                get { return CustomColor.Value; }

                set { CustomColor = new ModeParameter<IColor>("HighlightColor", value); }
            }

            /// <summary>
            /// Target scene object to be highlighted.
            /// </summary>
            [DataMember]
            [DisplayName("Objects")]
            public MultipleScenePropertyReference<IHighlightProperty> TargetObjects { get; set; }
        }
    }
}