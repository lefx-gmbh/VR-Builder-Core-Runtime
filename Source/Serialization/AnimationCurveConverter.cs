using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using VRBuilder.Core.Primitives;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Converter that serializes <see cref="IAnimationCurve"/> and deserializes <see cref="AnimationCurveData"/>.
    /// </summary>
    [NewtonsoftConverter]
    public class AnimationCurveConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                try
                {
                    var data = JObject.Load(reader);
                    return new AnimationCurveData(
                        ReadKeyFrames(data, serializer),
                        data["PreWrapMode"]?.Value<int>() ?? 0,
                        data["PostWrapMode"]?.Value<int>() ?? 0
                    );
                }
                catch (Exception ex)
                {
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Failed to deserialize {nameof(AnimationCurveData)} from JSON {SerializationLoggingHelper.FormatJsonLocation(ex)}.", ex));
                }
            }

            return new AnimationCurveData();
        }

        private static KeyframeData[] ReadKeyFrames(JObject data, JsonSerializer serializer)
        {
            var keys = data["Keyframes"]?.Value<JArray>();

            KeyframeData[] keyframes;
            if (keys != null)
                using (var keyReader = keys.CreateReader())
                    keyframes = serializer.Deserialize<KeyframeData[]>(keyReader) ?? Array.Empty<KeyframeData>();
            else
                keyframes = Array.Empty<KeyframeData>();
            return keyframes;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteNull();
                    return;
                case AnimationCurveData data:
                    new JObject
                    {
                        { "Keyframes", new JArray(data.Keyframes.Select(keyframe => JObject.FromObject(keyframe, serializer))) },
                        { "PreWrapMode", data.PreWrapMode },
                        { "PostWrapMode", data.PostWrapMode },
                    }.WriteTo(writer);
                    break;
                default:
                    ForwardingLogger.LogWarning(new JsonSerializationException($"Expected {nameof(AnimationCurveData)} but received {value.GetType().FullName}."));
                    break;
            }
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IAnimationCurve).IsAssignableFrom(objectType);
        }
    }
}