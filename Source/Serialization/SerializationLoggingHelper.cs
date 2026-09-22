using System;
using Newtonsoft.Json;

namespace VRBuilder.Core.Serialization
{
    internal static class SerializationLoggingHelper
    {
        internal static string FormatJsonLocation(Exception ex)
        {
            if (ex is JsonReaderException jre)
                return $"at line {jre.LineNumber}, position {jre.LinePosition} {(string.IsNullOrEmpty(jre.Path) ? "" : $", path '{jre.Path}'")}";
            return string.Empty;
        }
    }
}