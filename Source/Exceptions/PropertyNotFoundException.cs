// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Exceptions
{
    /// <summary>
    /// Thrown when a scene object does not provide a property of the requested type.
    /// </summary>
    public class PropertyNotFoundException : ProcessException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class with a custom error message.
        /// </summary>
        /// <param name="message">The message that describes the missing property.</param>
        public PropertyNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class describing which property type is missing on which scene object.
        /// </summary>
        /// <param name="sourceObject">The scene object that lacks the property.</param>
        /// <param name="missingType">The type of property that was not found.</param>
        public PropertyNotFoundException(ISceneObject sourceObject, Type missingType) : base($"SceneObject '{sourceObject}' does not contain a property of type '{missingType.Name}'")
        {
        }
    }
}