// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Utils.Audio;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Interface for the class playing sounds for the process, i.e. tts and play audio behaviors.
    /// </summary>
    public interface IAudioPlayer: ISceneObjectProperty
    {
        /// <summary>
        /// Gets a fallback audio source. Used only for backwards compatibility.
        /// </summary>
        IAudioData FallbackAudioSource { get; }

        /// <summary>
        /// True if currently playing audio.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Play the specified audio immediately with the set parameters.
        /// </summary>
        void PlayAudio(IAudioData audioData, float volume = 1f, float pitch = 1f);

        /// <summary>
        /// Stops playing audio.
        /// </summary>
        void StopAudio();

        /// <summary>
        /// Resets the player to its default settings.
        /// </summary>
        void ResetAudio();

        /// <summary>
        /// True if the used audio source is muted.
        /// </summary>
        bool IsMute { get; }
    }
}