// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.Exceptions
{
    /// <summary>
    /// Thrown when a referenced process entity cannot be found, for example a missing scene object or step.
    /// </summary>
    public class MissingEntityException : ProcessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingEntityException"/> class with a custom error message.
        /// </summary>
        /// <param name="message">The message that describes the missing entity.</param>
        public MissingEntityException(string message) : base(message)
        {
        }
    }
}