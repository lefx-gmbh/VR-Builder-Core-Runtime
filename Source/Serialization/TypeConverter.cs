// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Converts <see cref="Type"/> values to JSON and back without assembly qualification.
    ///
    /// Newtonsoft.Json's default behavior writes <see cref="Type.AssemblyQualifiedName"/> and reads it
    /// back through its internal <c>Type.GetType</c>, which bypasses the serializer's
    /// <see cref="ISerializationBinder"/>. That embeds the engine-specific assembly name (the Godot
    /// build compiles everything into "TinkerFlow", Unity uses "VRBuilder.Core" plus per-plugin
    /// assemblies), so a process file saved in one engine cannot be loaded in the other.
    ///
    /// This converter writes the bare <see cref="Type.FullName"/> and resolves on read the same way
    /// the V5 binder does: first against the ProcessEngine assembly (which contains the VRBuilder
    /// types in both engines), then against every loaded assembly (plugin types). Legacy
    /// assembly-qualified strings are still accepted, so existing files keep loading.
    /// </summary>
    [NewtonsoftConverter]
    internal class TypeConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    return;
                case Type type:
                    writer.WriteValue(type.FullName);
                    return;
                default:
                    throw new JsonSerializationException($"Expected {nameof(Type)} but received {value.GetType().FullName}.");
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            string typeName = reader.Value as string;
            if (string.IsNullOrEmpty(typeName))
            {
                throw new JsonSerializationException($"Expected a type name string.");
            }

            // Fast path: bare full names resolve against the calling assembly (the ProcessEngine
            // assembly in both engines). Legacy assembly-qualified names also resolve here when the
            // named assembly exists in this engine.
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            // Fallback: plugin types compiled into other assemblies, and legacy names whose assembly
            // does not exist in this engine (cross-engine files).
            string bareName = typeName.Split(',')[0].Trim();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(bareName);
                if (type != null)
                {
                    return type;
                }
            }

            throw new JsonSerializationException($"Could not resolve type '{typeName}'.");
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Type).IsAssignableFrom(objectType);
        }
    }
}