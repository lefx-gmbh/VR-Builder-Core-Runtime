using VRBuilder.Core.Registry;

namespace VRBuilder.Core.Input
{
    /// <summary>
    /// Defines the contract for the engine input controller, which sets up and loads the input
    /// actions used by the process.
    /// </summary>
    public interface IInputController : IService<IInputConfiguration>
    {
        /// <summary>
        /// Sets up the input actions so that they are ready to be processed.
        /// </summary>
        void SetupInputActions();

        /// <summary>
        /// Loads the input actions from the configured input action assets.
        /// </summary>
        void LoadInputActions();

        /// <summary>
        /// Returns whether the input controller uses a custom key binding asset.
        /// </summary>
        /// <returns><c>true</c> if a custom key binding asset is used; otherwise, <c>false</c>.</returns>
        bool UsesCustomKeyBindingAsset();
    }
}