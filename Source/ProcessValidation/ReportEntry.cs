// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.ProcessValidation
{
    /// <summary>
    /// Base report entry with all information available on editor Builder core.
    /// </summary>
    public class ReportEntry
    {
        /// <summary>
        /// ErrorCode to easily identifying the error.
        /// </summary>
        public readonly int Code;

        /// <summary>
        /// Priority level for this <see cref="ReportEntry"/>.
        /// </summary>
        public readonly ValidationErrorLevel ErrorLevel;

        /// <summary>
        /// Detailed description of the issue.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// Creates a new report entry with the given code, message, and error level.
        /// </summary>
        /// <param name="code">The error code identifying the issue.</param>
        /// <param name="message">A detailed description of the issue.</param>
        /// <param name="errorLevel">The priority level of the issue.</param>
        public ReportEntry(int code, string message, ValidationErrorLevel errorLevel)
        {
            Code = code;
            Message = message;
            ErrorLevel = errorLevel;
        }

        /// <summary>
        /// Creates a copy of an existing report entry.
        /// </summary>
        /// <param name="entry">The report entry to copy.</param>
        protected ReportEntry(ReportEntry entry)
        {
            Code = entry.Code;
            Message = entry.Message;
            ErrorLevel = entry.ErrorLevel;
        }
    }
}