using System;

namespace VRBuilder.Core.Attributes
{
    /// <summary>
    /// Fields or properties marked with this attribute are ignored in the step inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreInStepInspectorAttribute : Attribute
    {
    }
}