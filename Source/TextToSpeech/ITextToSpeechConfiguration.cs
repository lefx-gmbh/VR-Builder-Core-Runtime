// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using VRBuilder.Core.Registry;

namespace Source.TextToSpeech
{
    /// <summary>
    /// Configuration for text-to-speech generation, covering the active provider, voice profiles and the audio file format.
    /// </summary>
    public interface ITextToSpeechConfiguration : IServiceConfiguration
    {
        /// <summary>
        /// Supported audio file types that can be used to generate TTS-files by the specific provider.
        /// </summary>
        public enum SupportedAudioType
        {
            /// <summary>
            /// Default type that works on every platform
            /// </summary>
            WAV,

            /// <summary>
            /// MPEG audio format.
            /// </summary>
            MP3,

            /// <summary>
            /// OGG audio format.
            /// </summary>
            OGG
        }

        /// <summary>
        /// Current active used audio type to generate text-to-speech files.
        /// </summary>
        public SupportedAudioType SelectedAudioType { get; set; }

        /// <summary>
        /// StreamingAsset directory name that is used to load/save audio files.
        /// </summary>
        string StreamingAssetCacheDirectoryName { get; set; }

        /// <summary>
        /// Invoked when the text-to-speech provider changes.
        /// </summary>
        public event Action ProviderChanged;

        /// <summary>
        /// Invoked when the voice profiles are changed.
        /// </summary>
        public event Action VoiceProfilesChanged;
    }
}