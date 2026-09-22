namespace VRBuilder.Core.Highlighting
{
    /// <summary>
    /// Adds highlighting functionality to a GameObject with Renderers.
    /// </summary>
    public interface IHighlighter
    {
        /// <summary>
        /// Returns true if there is this object is currently being highlighted.
        /// </summary>
        bool IsHighlighting { get; }

        /// <summary>
        /// Starts highlighting this object.
        /// </summary>
        /// <param name="highlightId">Material ID to be applied as highlight.</param>
        void StartHighlighting(string highlightId);

        /// <summary>
        /// Stops highlighting this object.
        /// </summary>
        void StopHighlighting();

        /// <summary>
        /// Returns the ID of the Highlight Material
        /// </summary>
        /// <returns>string id</returns>
        string GetHighlightMaterialId();
    }
}
