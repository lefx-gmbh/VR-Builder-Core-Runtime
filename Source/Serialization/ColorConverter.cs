// Copyright (c) 2013-2019 Innoactive GmbH
// Modifications copyright (c) 2021-2026 MindPort GmbH
// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRBuilder.Core.Primitives;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Converts Unity color into json and back.
    /// </summary>
    [NewtonsoftConverter]
    internal class ColorConverter : JsonConverter
    {
        /// <inheritDoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    return;
                case IColor color:
                    new JObject
                    {
                        { "r", color.R },
                        { "g", color.G },
                        { "b", color.B },
                        { "a", color.A },
                    }.WriteTo(writer);
                    break;
                default:
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Expected {nameof(IColor)} but received {value.GetType().FullName}."));
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

                    var r = data["r"]?.Value<float>() ?? 1;
                    var g = data["g"]?.Value<float>() ?? 0;
                    var b = data["b"]?.Value<float>() ?? 1;
                    var a = data["a"]?.Value<float>() ?? 1;

                    return new ColorData(r, g, b, a);
                }
                catch (Exception ex)
                {
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Failed to deserialize {nameof(ColorData)} from JSON {SerializationLoggingHelper.FormatJsonLocation(ex)}.", ex));
                }
            }

            return new ColorData(1, 0, 1);
        }


        /// <inheritDoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IColor).IsAssignableFrom(objectType);
        }
    }
}