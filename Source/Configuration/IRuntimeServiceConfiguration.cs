using VRBuilder.Core.Registry;

namespace VRBuilder.Core.Configuration
{
    /// <summary>
    /// Service configuration for the runtime, adding the location of the selected process's streaming assets.
    /// </summary>
    public interface IRuntimeServiceConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// The path under the streaming assets folder where the selected process is stored.
        /// </summary>
        public string SelectedProcessStreamingAssetsPath { get; set; }
    }
}