using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRBuilder.Core.Primitives;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Converter that serializes <see cref="IKeyframe"/> and deserializes <see cref="KeyframeData"/>.
    /// </summary>
    [NewtonsoftConverter]
    public class KeyframeConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    return;
                case IKeyframe keyframe:
                    new JObject
                    {
                        { "Time", keyframe.Time },
                        { "Value", keyframe.Value },
                        { "InTangent", keyframe.InTangent },
                        { "OutTangent", keyframe.OutTangent },
                        { "WeightedMode", keyframe.WeightedMode },
                    }.WriteTo(writer);
                    break;
                default:
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Expected {nameof(IKeyframe)} but received {value.GetType().FullName}."));
                    break;
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                try
                {
                    var data = JObject.Load(reader);

                    float time = data["Time"]?.Value<float>() ?? 0;
                    float value = data["Value"]?.Value<float>() ?? 0;
                    float inTangent = data["InTangent"]?.Value<float>() ?? 0;
                    float outTangent = data["OutTangent"]?.Value<float>() ?? 0;
                    int weightedMode = data["WeightedMode"]?.Value<int>() ?? 0;

                    return new KeyframeData(time, value, inTangent, outTangent, weightedMode);
                }
                catch (Exception ex)
                {
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Failed to deserialize {nameof(KeyframeData)} from JSON {SerializationLoggingHelper.FormatJsonLocation(ex)}.", ex));
                }
            }

            return new KeyframeData();
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IKeyframe).IsAssignableFrom(objectType);
        }
    }
}