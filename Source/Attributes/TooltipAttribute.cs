using System;

namespace VRBuilder.Core.Attributes
{
    /// <summary>
    /// Tooltip of process entity's property or field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
    public class DisplayTooltipAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="DisplayTooltipAttribute"/>.
        /// </summary>
        /// <param name="tooltip">The tooltip of the process entity's property or field.</param>
        public DisplayTooltipAttribute(string tooltip)
        {
            Tooltip = tooltip;
        }

        /// <summary>
        /// Tooltip of the process entity's property or field.
        /// </summary>
        public string Tooltip { get; private set; }
    }
}