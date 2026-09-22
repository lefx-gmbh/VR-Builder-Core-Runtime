// Copyright (c) 2021-2026 MindPort GmbH

using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VRBuilder.Core.Serialization;

namespace VRBuilder.Core.Serialization.NewtonsoftJson
{
    /// <summary>
    /// Maps core's engine-agnostic, serializer-agnostic serialization attributes
    /// (<see cref="SerializationConstructorAttribute"/>, <see cref="SerializedNameAttribute"/>)
    /// onto Newtonsoft.Json's own contract model, so core types can be annotated without
    /// referencing Newtonsoft.Json directly.
    /// </summary>
    internal class VRBuilderContractResolver : DefaultContractResolver
    {
        /// <inheritdoc/>
        protected override JsonObjectContract CreateObjectContract(System.Type objectType)
        {
            JsonObjectContract contract = base.CreateObjectContract(objectType);

            ConstructorInfo constructor = objectType
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(candidate => candidate.GetCustomAttribute<SerializationConstructorAttribute>() != null);

            if (constructor != null)
            {
                contract.OverrideCreator = args => constructor.Invoke(args);
                contract.CreatorParameters.Clear();

                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    JsonProperty matchingMemberProperty = contract.Properties
                        .FirstOrDefault(candidate => string.Equals(candidate.UnderlyingName, parameter.Name, System.StringComparison.OrdinalIgnoreCase));
                    JsonProperty property = CreatePropertyFromConstructorParameter(matchingMemberProperty, parameter);
                    if (property != null)
                    {
                        contract.CreatorParameters.AddProperty(property);
                    }
                }
            }

            return contract;
        }

        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            SerializedNameAttribute attribute = member.GetCustomAttribute<SerializedNameAttribute>();
            if (attribute != null)
            {
                property.PropertyName = attribute.Name;
            }

            return property;
        }
    }
}
