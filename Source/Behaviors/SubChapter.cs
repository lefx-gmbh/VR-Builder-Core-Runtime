using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VRBuilder.Core.Behaviors
{
    /// <summary>
    /// Represents a nested chapter that can be optionally skipped.
    /// </summary>
    [DataContract(IsReference = true)]
    public class SubChapter
    {
        /// <summary>
        /// Creates a required (non-optional) sub-chapter.
        /// </summary>
        /// <param name="chapter">The chapter to execute.</param>
        public SubChapter(IChapter chapter) : this(chapter, false)
        {
        }

        /// <summary>
        /// Creates a sub-chapter with the given optionality, used by the JSON deserializer.
        /// </summary>
        /// <param name="chapter">The chapter to execute.</param>
        /// <param name="isOptional">If <c>true</c>, the chapter can be skipped by the user or the process.</param>
        [JsonConstructor]
        public SubChapter(IChapter chapter, bool isOptional)
        {
            Chapter = chapter;
            IsOptional = isOptional;
        }

        /// <summary>
        /// The chapter to execute.
        /// </summary>
        [DataMember]
        public IChapter Chapter { get; }

        /// <summary>
        /// If true, the chapter can be skipped by the user or the process.
        /// </summary>
        [DataMember]
        public bool IsOptional { get; set; }
    }
}