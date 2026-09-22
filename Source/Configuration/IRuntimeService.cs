using System;
using System.Threading.Tasks;
using VRBuilder.Core.Registry;
using VRBuilder.Core.Utils.Logging;

namespace VRBuilder.Core.Configuration
{
    /// <summary>
    /// Provides access to the runtime configuration and process loading for the current application session.
    /// </summary>
    public interface IRuntimeService : IService<IRuntimeServiceConfiguration>
    {
        /// <summary>
        /// The name of the selected process, or <c>null</c> if none is selected.
        /// </summary>
        string? SelectedProcess { get; set; }

        /// <summary>
        /// The file name of the process manifest, or <c>null</c> if none is used.
        /// </summary>
        string? ManifestFileName { get; set; }

        /// <summary>
        /// The streaming-assets path of the selected process.
        /// </summary>
        string SelectedProcessStreamingAssetsPath { get; set; }

        /// <summary>
        /// The runtime configurator of the current session.
        /// </summary>
        IRuntimeConfigurator Configurator { get; set; }

        /// <summary>
        /// Configuration of which lifecycle events should be logged.
        /// </summary>
        ILifeCycleLoggingConfiguration LifeCycleLogging { get; }

        /// <summary>
        /// Loads the process from the given path.
        /// </summary>
        /// <param name="path">The path to the process file.</param>
        /// <returns>The loaded process.</returns>
        public Task<IProcess> LoadProcess(string path);

        /// <summary>
        /// Raised when the selected process changes.
        /// </summary>
        public event Action<string?> SelectedProcessChanged;
    }
}