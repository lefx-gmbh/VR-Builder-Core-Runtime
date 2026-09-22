using System;
using VRBuilder.Core.Utils.Audio;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Property for the connection of a scene-based audio player.
    /// </summary>
    public interface IAudioSourceProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Called when a new text to speech is played.
        /// </summary>
        event Action<(IAudioData, string)> PlayTextToSpeech;
        
        /// <summary>
        /// Called when a new text to speech is played.
        /// </summary>
        event Action<(IAudioData, string)> EndTextToSpeech;

        /// <summary>
        /// Get the selected audio player.
        /// </summary>
        public IAudioData AudioPlayer { get; }
    }
}
