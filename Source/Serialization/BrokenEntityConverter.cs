using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRBuilder.Core.Behaviors;
using VRBuilder.Core.Conditions;

namespace VRBuilder.Core.Serialization
{
    /// <summary>
    /// Converts behavior and condition JSON, falling back to inspectable placeholders when deserialization fails.
    /// </summary>
    internal class BrokenEntityConverter : JsonConverter
    {
        /// <summary>
        /// Gets a value indicating that placeholder serialization uses the default Json.NET writer.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// Deserializes a behavior or condition and preserves failed items as broken entities.
        /// </summary>
        /// <param name="reader">The reader positioned at the entity JSON.</param>
        /// <param name="objectType">The requested behavior or condition interface.</param>
        /// <param name="existingValue">The existing value supplied by Json.NET.</param>
        /// <param name="serializer">The serializer used to resolve references and concrete types.</param>
        /// <returns>The deserialized entity or a broken placeholder containing the original JSON.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            string path = reader.Path;
            JObject json = JObject.Load(reader);

            try
            {
                string? reference = json.Value<string>("$ref");
                if (reference != null)
                {
                    object referencedEntity = serializer.ReferenceResolver.ResolveReference(serializer, reference);
                    if (objectType.IsInstanceOfType(referencedEntity))
                    {
                        return referencedEntity;
                    }

                    throw new JsonSerializationException($"Reference '{reference}' does not point to a {objectType.Name}.");
                }

                Type concreteType = ResolveType(json, serializer);
                return json.ToObject(concreteType, serializer)
                       ?? throw new JsonSerializationException($"Could not deserialize {concreteType.FullName}.");
            }
            catch (Exception exception)
            {
                string error = $"{exception.Message} Item path '{path}'.";
                string rawJson = json.ToString(Formatting.Indented);

                if (objectType == typeof(IBehavior))
                {
                    return new BrokenBehavior(error, rawJson);
                }

                return new BrokenCondition(error, rawJson);
            }
        }

        /// <summary>
        /// Rejects direct writes because the default Json.NET writer serializes recovered placeholders.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The value being serialized.</param>
        /// <param name="serializer">The active serializer.</param>
        /// <exception cref="InvalidOperationException">Always thrown because this converter is read-only.</exception>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Determines whether the requested type is a recoverable behavior or condition interface.
        /// </summary>
        /// <param name="objectType">The requested object type.</param>
        /// <returns><c>true</c> for <see cref="IBehavior"/> and <see cref="ICondition"/>; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IBehavior) || objectType == typeof(ICondition);
        }

        /// <summary>
        /// Resolves the concrete type declared by an entity's <c>$type</c> metadata.
        /// </summary>
        /// <param name="json">The entity JSON containing type metadata.</param>
        /// <param name="serializer">The serializer providing the configured type binder.</param>
        /// <returns>The resolved concrete entity type.</returns>
        /// <exception cref="JsonSerializationException">Thrown when the entity has no <c>$type</c> metadata.</exception>
        private static Type ResolveType(JObject json, JsonSerializer serializer)
        {
            string? qualifiedTypeName = json.Value<string>("$type");
            if (string.IsNullOrEmpty(qualifiedTypeName))
            {
                throw new JsonSerializationException("Behavior or condition has no $type metadata.");
            }

            int separatorIndex = qualifiedTypeName.IndexOf(',');
            string typeName = separatorIndex < 0 ? qualifiedTypeName : qualifiedTypeName.Substring(0, separatorIndex).Trim();
            string? assemblyName = separatorIndex < 0 ? null : qualifiedTypeName.Substring(separatorIndex + 1).Trim();
            return serializer.SerializationBinder.BindToType(assemblyName, typeName);
        }
    }
}