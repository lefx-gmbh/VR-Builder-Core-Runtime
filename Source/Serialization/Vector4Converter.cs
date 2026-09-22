// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRBuilder.Core.Primitives;
using VRBuilder.Core.Properties;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Converts Vector4 into json and back.
    /// </summary>
    [NewtonsoftConverter]
    internal class Vector4Converter : JsonConverter
    {
        /// <inheritDoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    return;
                case IVector4 vec:
                {
                    new JObject
                    {
                        { "x", vec.X },
                        { "y", vec.Y },
                        { "z", vec.Z },
                        { "w", vec.W }
                    }.WriteTo(writer);
                    break;
                }
                default:
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Expected {nameof(IVector4)} but received {value.GetType().FullName}."));
                    break;
            }
        }

        /// <inheritDoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                try
                {
                    var data = JObject.Load(reader);

                    var x = data["x"]?.Value<float>() ?? 0;
                    var y = data["y"]?.Value<float>() ?? 0;
                    var z = data["z"]?.Value<float>() ?? 0;
                    var w = data["w"]?.Value<float>() ?? 0;

                    return new Vector4Data(x, y, z, w);
                }
                catch (Exception ex)
                {
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Failed to deserialize {nameof(Vector4Data)} from JSON {SerializationLoggingHelper.FormatJsonLocation(ex)}.", ex));
                }
            }

            return new Vector4Data(0, 0, 0, 0);
        }

        /// <inheritDoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IVector4) == objectType;
        }
    }
}
