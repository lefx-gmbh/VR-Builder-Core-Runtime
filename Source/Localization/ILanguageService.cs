// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Globalization;
using VRBuilder.Core.Registry;

namespace VRBuilder.Core.Localization
{
    /// <summary>
    /// Language service to load current language-based settings from the runtime configuration.
    /// </summary>
    public interface ILanguageService : IService<ILanguageConfiguration>
    {
        /// <summary>
        /// String localization table used by the current process.
        /// </summary>
        string ProcessStringLocalizationTable { get; }

        /// <summary>
        /// Gets the active selected or default language.
        /// </summary>
        CultureInfo ActiveOrDefaultLocale { get; set; }

        /// <summary>
        /// Return the region code of the active or default language.
        /// </summary>
        string ActiveOrDefaultRegionCode { get; set; }

        /// <summary>
        /// Current selected application language.
        /// </summary>
        string ApplicationLanguage { get; set; }

        /// <summary>
        /// Get Locale object from a language or language code string.
        /// </summary>
        /// <param name="languageOrCode">The language or language code string.</param>
        /// <returns>The Locale object corresponding to the language code string or NULL.</returns>
        public CultureInfo GetCultureInfoFromString(string languageOrCode);

        /// <summary>
        /// Convert natural language name to two-letters ISO code.
        /// </summary>
        /// <param name="language">String with natural language name or two-letters ISO code.</param>
        /// <param name="result">
        /// If <paramref name="language"/> is already in two-letters ISO code, simply returns it.
        /// If <paramref name="language"/> is a natural language name, returns two-symbol code.
        /// Otherwise, returns null.
        /// </param>
        /// <returns>Return true if the operation successful.</returns>
        public bool TryConvertToTwoLetterIsoCode(string language, out string result);

        /// <summary>
        /// Try to localize a step name if used as a key in a localization table.
        /// </summary>
        /// <param name="step">Reference of the current step.</param>
        /// <param name="localizationTable">Name of the location of the localized key.</param>
        /// <param name="locale">Language to localize the step into.</param>
        /// <returns></returns>
        public string GetLocalizedStepName(IStep step, string localizationTable, CultureInfo locale);

        /// <summary>
        /// Try to localize a chapter name if used as a key in a localization table.
        /// </summary>
        /// <param name="chapter">Reference of the current chapter.</param>
        /// <param name="localizationTable">Name of the location of the localized key.</param>
        /// <param name="locale">Language to localize the chapter into.</param>
        /// <returns></returns>
        public string GetLocalizedChapterName(IChapter chapter, string localizationTable, CultureInfo locale);

        /// <summary>
        /// Try to get the localized string for a key and in a table with the currently selcted locale.
        /// </summary>
        /// <param name="localizationKey">Key of the localized string.</param>
        /// <param name="localizationTable">Name of the location of the localized key.</param>
        /// <returns>Returns the localized string with the given key and location.</returns>
        public string GetLocalizedString(string localizationKey, string localizationTable);

        /// <summary>
        /// Try to get the localized string for a key and in a table with a custom locale.
        /// </summary>
        /// <param name="localizationKey">Key of the localized string.</param>
        /// <param name="localizationTable">Name of the location of the localized key.</param>
        /// <param name="locale">Custom language used inside the engines.</param>
        /// <returns>Returns the localized string with the given key, location, and custom language.</returns>
        public string GetLocalizedString(string localizationKey, string localizationTable, CultureInfo locale);

        /// <summary>
        /// Try to get the localized string for a key and in a table with a custom locale.
        /// </summary>
        /// <param name="localizationKey">Key of the localized string.</param>
        /// <returns>Returns the localized string with the given key, location, and custom language.</returns>
        public string GetLocalizedString(string localizationKey);

        /// <summary>
        /// Check if <paramref name="language"/> is two-letter ISO code.
        /// </summary>
        /// <param name="language">Input string checked if it's ISO code.</param>
        /// <returns>Returns true if the input string is a two-letter ISO code.</returns>
        public bool IsTwoLettersIsoCode(string language);

        /// <summary>
        /// Helps to convert strings with full language names like "English" to a two-letter ISO language code.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="languageName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="languageName"/> is not natural language name.</exception>
        /// <returns>The two-letter ISO code from the given language name. If it cannot parse the string, it returns null.</returns>
        public string ConvertNaturalLanguageNameToTwoLetterIsoCode(string languageName);
    }
}