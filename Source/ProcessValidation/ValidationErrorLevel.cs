// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

namespace VRBuilder.Core.ProcessValidation
{
    /// <summary>
    /// Error level of the problem found while validating.
    /// </summary>
    public enum ValidationErrorLevel
    {
        /// <summary>
        /// A suggestion that does not affect the correctness of the process.
        /// </summary>
        HINT,

        /// <summary>
        /// A potential issue that may cause unintended behavior.
        /// </summary>
        WARNING,

        /// <summary>
        /// An issue that prevents the affected part from behaving correctly.
        /// </summary>
        ERROR,

        /// <summary>
        /// An issue that makes the process unusable.
        /// </summary>
        FATAL
    }
}