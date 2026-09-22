// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Engine-agnostic audio clip representation that carries raw PCM samples and the playback
    /// metadata required to interpret them, so clips can cross the Unity/Godot boundary.
    /// </summary>
    public interface IAudioClip
    {
        /// <summary>
        /// Gets the raw (uncompressed) PCM audio samples; their meaning is defined by
        /// <see cref="Frequency"/> and <see cref="Channels"/>.
        /// </summary>
        byte[] RawAudioData { get; }

        /// <summary>Gets the sample rate of the audio data in hertz.</summary>
        int Frequency { get; }

        /// <summary>Gets the number of audio channels (1 = mono, 2 = stereo).</summary>
        int Channels { get; }

        /// <summary>Gets the display or identifier name of the clip.</summary>
        string Name { get; }
    }
}