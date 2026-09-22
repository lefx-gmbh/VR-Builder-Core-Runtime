// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Overrides the serialized name of the annotated member.
    ///
    /// Engine-agnostic and serializer-agnostic replacement for Newtonsoft.Json's
    /// <c>JsonPropertyAttribute</c> - core types are annotated with this instead of
    /// referencing Newtonsoft.Json directly. Concrete serializers (e.g. the Newtonsoft-based
    /// one) map this attribute to their own property-naming mechanism.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SerializedNameAttribute : Attribute
    {
        /// <summary>
        /// The name to use when serializing the annotated member.
        /// </summary>
        public string Name { get; }

        public SerializedNameAttribute(string name)
        {
            Name = name;
        }
    }
}
