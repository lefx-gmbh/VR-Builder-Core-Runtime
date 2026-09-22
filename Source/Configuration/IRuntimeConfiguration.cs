namespace VRBuilder.Core.Configuration
{
    /// <summary>
    /// Holds the runtime selection of which process is loaded and where its manifest is located.
    /// Used by <see cref="IRuntimeConfigurator"/> to drive which process the runtime runs.
    /// </summary>
    public interface IRuntimeConfiguration
    {
        /// <summary>
        /// The name or identifier of the process that is currently selected to run.
        /// <c>null</c> means no process has been selected yet.
        /// </summary>
        string? SelectedProcess { get; set; }

        /// <summary>
        /// The file name of the manifest that describes the selected process.
        /// </summary>
        string ManifestFileName { get; set; }
    }
}