namespace VRBuilder.Core.Setup
{
    /// <summary>
    /// Define a set of raycast/interaction layers for a specific use.
    /// </summary>
    public enum LayerSet
    {
        /// <summary>
        /// No layers are included.
        /// </summary>
        None,

        /// <summary>
        /// Layers used for direct and ray interactions.
        /// </summary>
        Interaction,

        /// <summary>
        /// Layers used for teleportation.
        /// </summary>
        Teleportation,

        /// <summary>
        /// Layers used for UI interaction.
        /// </summary>
        UI,
    }
}