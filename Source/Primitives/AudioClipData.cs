// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.Serialization;

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Engine-agnostic, serializable representation of an audio clip.
    /// Stores the raw PCM sample data and its format so audio can be recreated in any engine.
    /// </summary>
    [DataContract]
    public struct AudioClipData : IAudioClip
    {
        /// <summary>The raw PCM audio sample data.</summary>
        [DataMember]
        public byte[] RawAudioData { readonly get; set; }

        /// <summary>The sample rate of the audio clip in Hz.</summary>
        [DataMember]
        public int Frequency { readonly get; set; }

        /// <summary>The number of audio channels (1 for mono, 2 for stereo).</summary>
        [DataMember]
        public int Channels { readonly get; set; }

        /// <summary>An optional display name for the audio clip.</summary>
        [DataMember]
        public string Name { readonly get; set; }

        /// <summary>
        /// Initializes a new <see cref="AudioClipData"/> with the specified audio data and format.
        /// </summary>
        /// <param name="rawAudioData">The raw PCM audio sample data.</param>
        /// <param name="frequency">The sample rate of the audio clip in Hz.</param>
        /// <param name="channels">The number of audio channels (1 for mono, 2 for stereo).</param>
        /// <param name="name">An optional display name for the audio clip. Defaults to <c>null</c>.</param>
        public AudioClipData(byte[] rawAudioData, int frequency, int channels, string name = null)
        {
            RawAudioData = rawAudioData;
            Frequency = frequency;
            Channels = channels;
            Name = name;
        }
    }
}