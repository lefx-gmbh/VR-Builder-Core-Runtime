// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System.Threading.Tasks;
using VRBuilder.Core.Primitives;
using VRBuilder.Core.TextToSpeech.Configuration;
using VRBuilder.Core.TextToSpeech.Utils;

namespace VRBuilder.Core.TextToSpeech.Providers
{
    /// <summary>
    /// TextToSpeechProvider allows converting text to AudioClips.
    /// </summary>
    public interface ITextToSpeechProvider
    {
        /// <summary>
        /// Used for setting the config file.
        /// </summary>
        void SetConfig(ITextToSpeechProviderConfiguration providerConfiguration);

        /// <summary>
        /// Loads the AudioClip file for the given text.
        /// </summary>
        /// <param name="requestFileLocator">Properties containing all information about the text-to-speech audio data.</param>
        /// <returns>ready to play Audioclip</returns>
        Task<IAudioClip> ConvertTextToSpeech(ITextToSpeechFileLocator requestFileLocator);

        /// <summary>
        /// Load config while editor- and runtime
        /// </summary>
        /// <returns>Returns configuration for the provider if successful</returns>
        public ITextToSpeechProviderConfiguration LoadConfig();
    }
}
