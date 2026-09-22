namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Engine-agnostic color.
    /// Replaces UnityEngine.Color in interfaces.
    /// </summary>
    public interface IColor
    {
        /// <summary>Red component (0..1).</summary>
        float R { get; }

        /// <summary>Green component (0..1).</summary>
        float G { get; }

        /// <summary>Blue component (0..1).</summary>
        float B { get; }

        /// <summary>Alpha component (0..1).</summary>
        float A { get; }
    }
}
