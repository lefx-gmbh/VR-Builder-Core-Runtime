// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.Exceptions
{
    /// <summary>
    /// Thrown when a process or its parts are used in a state that does not permit the requested operation.
    /// </summary>
    public class InvalidStateException : ProcessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStateException"/> class with a custom error message.
        /// </summary>
        /// <param name="message">The message that describes the invalid state.</param>
        public InvalidStateException(string message) : base(message)
        {
        }
    }
}