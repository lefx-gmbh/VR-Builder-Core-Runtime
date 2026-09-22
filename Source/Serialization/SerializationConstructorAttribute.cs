// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Marks the constructor a serializer should use to create an instance of the annotated type.
    ///
    /// Engine-agnostic and serializer-agnostic replacement for Newtonsoft.Json's
    /// <c>JsonConstructorAttribute</c> - core types are annotated with this instead of
    /// referencing Newtonsoft.Json directly. Concrete serializers (e.g. the Newtonsoft-based
    /// one) map this attribute to their own constructor-selection mechanism.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class SerializationConstructorAttribute : Attribute
    {
    }
}
