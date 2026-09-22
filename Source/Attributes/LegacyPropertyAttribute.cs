using System;

namespace VRBuilder.Core.Attributes
{
    /// <summary>
    /// Attribute for an obsolete property which has been replaced by a different property.
    /// </summary>
    public class LegacyPropertyAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="LegacyPropertyAttribute"/>.
        /// </summary>
        /// <param name="newPropertyName">The name of the property replacing this obsolete property.</param>
        public LegacyPropertyAttribute(string newPropertyName)
        {
            NewPropertyName = newPropertyName;
        }

        /// <summary>
        /// Name of the property replacing this obsolete property.
        /// </summary>
        public string NewPropertyName { get; private set; }
    }
}