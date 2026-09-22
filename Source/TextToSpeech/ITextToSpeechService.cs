// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Registry;
using VRBuilder.Core.TextToSpeech.Providers;
using VRBuilder.Core.TextToSpeech.Utils;

namespace Source.TextToSpeech
{
    /// <summary>
    /// Provides access to the configured text-to-speech provider and to file path handling for generated speech.
    /// </summary>
    public interface ITextToSpeechService : IService<ITextToSpeechConfiguration>
    {
        /// <summary>
        /// The provider used to generate text-to-speech files, or the default provider when none is explicitly selected.
        /// </summary>
        public ITextToSpeechProvider DefaultOrActiveTextToSpeechProvider { get; set; }

        /// <summary>
        /// The text-to-speech configuration used by this service.
        /// </summary>
        public ITextToSpeechConfiguration Configuration { get; set; }

        /// <summary>
        /// Get a full path based on a <paramref name="fileLocator"/> information to produce speech from, and create a directory for that.
        /// </summary>
        /// <param name="fileLocator">Locator information of the text-to-speech file.</param>
        /// <returns>The full file path for the text-to-speech file.</returns>
        public string PrepareFilepathForTextToSpeechFile(ITextToSpeechFileLocator fileLocator);
    }
}