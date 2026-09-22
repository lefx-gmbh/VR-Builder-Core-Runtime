using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VRBuilder.Core.Attributes;
using VRBuilder.Core.Configuration.Modes;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Preserves a behavior that could not be deserialized so its source JSON remains inspectable.
    /// </summary>
    /// <remarks>
    /// Do not use this class as a normal behavior. It's just a fallback for importing processes!
    /// </remarks>
    [DataContract(IsReference = true)]
    internal class BrokenBehavior : Behavior<BrokenBehavior.EntityData>
    {
        /// <summary>
        /// Creates an empty placeholder for JSON deserialization.
        /// </summary>
        [JsonConstructor]
        internal BrokenBehavior()
        {
        }

        /// <summary>
        /// Creates a placeholder containing the failed behavior's diagnostic information.
        /// </summary>
        /// <param name="error">The error that prevented deserialization.</param>
        /// <param name="rawJson">The original behavior JSON.</param>
        internal BrokenBehavior(string error, string rawJson)
        {
            Data.Error = error;
            Data.RawJson = rawJson;
        }

        /// <summary>
        /// Prevents a process containing this placeholder from running.
        /// </summary>
        /// <param name="mode">The process mode being configured.</param>
        /// <exception cref="InvalidOperationException">Always thrown because the original behavior is unavailable.</exception>
        public override void Configure(IMode mode)
        {
            throw new InvalidOperationException($"Cannot run a process containing a broken behavior. {Data.Error}");
        }

        /// <summary>
        /// Stores the failed behavior's error and original JSON.
        /// </summary>
        [DisplayName("Broken Behavior")]
        [DataContract(IsReference = true)]
        internal class EntityData : IBehaviorData
        {
            [DataMember]
            [DisplayName("Error")]
            public string Error { get; set; } = "";

            [DataMember]
            [DisplayName("Raw JSON")]
            public string RawJson { get; set; } = "";

            public Metadata Metadata { get; set; } = new Metadata();

            [IgnoreDataMember]
            public string Name => "Broken behavior";
        }
    }
}