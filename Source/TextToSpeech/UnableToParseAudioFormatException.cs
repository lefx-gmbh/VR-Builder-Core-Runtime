// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;

namespace VRBuilder.Core.TextToSpeech
{
    /// <summary>
    /// Thrown when audio data cannot be interpreted as the expected audio format.
    /// </summary>
    public class UnableToParseAudioFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnableToParseAudioFormatException"/> class with a custom error message.
        /// </summary>
        /// <param name="msg">The message that describes the audio format problem.</param>
        public UnableToParseAudioFormatException(string msg) : base(msg)
        {
        }
    }
}