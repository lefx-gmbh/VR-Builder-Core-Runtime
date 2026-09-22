// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.Serialization;
using VRBuilder.Core.Utils.Audio;

namespace VRBuilder.Core.Primitives
{
    /// <summary>
    /// Serializable audio data wrapping an <see cref="IAudioClip"/> and tracking its loading state.
    /// The default instance wraps an empty clip with no playable audio.
    /// </summary>
    [DataContract]
    public class AudioData : IAudioData
    {
        private readonly IAudioClip audioClip;
        private readonly string clipName;
        private readonly bool hasAudio;
        private string clipData;
        private bool isLoading;
        private bool isReady;

        /// <summary>
        /// Creates an empty <see cref="AudioData"/> with no audio.
        /// </summary>
        public AudioData()
        {
            audioClip = new AudioClipData(new byte[] { }, 0, 0);
            isReady = true;
            isLoading = false;
        }

        /// <summary>
        /// Creates <see cref="AudioData"/> for the given clip, which is initially marked as loading.
        /// </summary>
        /// <param name="audioClip">The audio clip data.</param>
        /// <param name="clipName">The name of the clip.</param>
        public AudioData(AudioClipData audioClip, string clipName)
        {
            this.audioClip = audioClip;
            this.clipName = clipName;
            isReady = false;
            isLoading = true;
        }

        /// <summary>
        /// <c>true</c> if the clip contains non-empty raw audio data.
        /// </summary>
        public bool HasAudio => audioClip is { RawAudioData: { Length: > 0 } };

        /// <summary>
        /// <c>true</c> while the clip is being loaded.
        /// </summary>
        public bool IsLoading => isLoading;

        /// <summary>
        /// <c>true</c> once the clip is ready to be played.
        /// </summary>
        public bool IsReady => isReady;

        /// <summary>
        /// Serialized clip data.
        /// </summary>
        public string ClipData
        {
            get => clipData;
            set => clipData = value;
        }

        /// <summary>
        /// The audio clip, or an empty clip when no audio is present.
        /// </summary>
        public IAudioClip AudioClip => audioClip;

        /// <summary>
        /// Marks the audio data as loaded and ready.
        /// </summary>
        public void Initialize()
        {
            isReady = true;
            isLoading = false;
        }

        /// <summary>
        /// Returns <c>true</c> if the audio data contains no playable audio.
        /// </summary>
        /// <returns><c>true</c> if there is no audio, otherwise <c>false</c>.</returns>
        public bool IsEmpty()
        {
            return !HasAudio;
        }
    }
}