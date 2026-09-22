// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;

namespace VRBuilder.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a required mode is missing or cannot be resolved.
    /// </summary>
    public class MissingModeException : ProcessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingModeException"/> class.
        /// </summary>
        public MissingModeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingModeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingModeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingModeException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public MissingModeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}