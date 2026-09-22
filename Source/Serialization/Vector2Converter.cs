// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRBuilder.Core.Primitives;
using VRBuilder.Core.Properties;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Converts <see cref="IVector2"/> implementations (typically <see cref="Vector2Data"/>) to JSON and back.
    /// </summary>
    [NewtonsoftConverter]
    internal class Vector2Converter : JsonConverter
    {
        /// <inheritDoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    return;
                case IVector2 vec:
                {
                    new JObject
                    {
                        { "x", vec.X },
                        { "y", vec.Y }
                    }.WriteTo(writer);
                    break;
                }
                default:
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Expected {nameof(IVector2)} but received {value.GetType().FullName}."));
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

                    return new Vector2Data(x, y);
                }
                catch (Exception ex)
                {
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Failed to deserialize {nameof(Vector2Data)} from JSON {SerializationLoggingHelper.FormatJsonLocation(ex)}.", ex));
                }
            }

            return new Vector2Data(0, 0);
        }

        /// <inheritDoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IVector2).IsAssignableFrom(objectType);
        }
    }
}