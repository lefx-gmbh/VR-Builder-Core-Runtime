namespace VRBuilder.Core.Configuration
{
    /// <summary>
    /// Implemented by components that expose the runtime configuration used to start a process.
    /// </summary>
    public interface IRuntimeConfigurator
    {
        /// <summary>
        /// The configuration that determines which process is selected and where its manifest is located.
        /// </summary>
        IRuntimeConfiguration RuntimeConfiguration { get; set; }
    }
}