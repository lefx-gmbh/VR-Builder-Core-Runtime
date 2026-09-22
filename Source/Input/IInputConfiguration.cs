using VRBuilder.Core.Registry;

namespace VRBuilder.Core.Input
{
    /// <summary>
    /// Configuration for the <see cref="IInputController"/>, defining the input action assets used
    /// for default and customized key bindings.
    /// </summary>
    public interface IInputConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// Default input action asset which is used when no customization of key bindings are done.
        /// Should be stored inside the VR Builder package.
        /// </summary>
        public string DefaultInputActionAssetPath { get; }

        /// <summary>
        /// Custom InputActionAsset path which is used when key bindings are modified.
        /// Should be stored in project path.
        /// </summary>
        public string CustomInputActionAssetPath { get; }
    }
}