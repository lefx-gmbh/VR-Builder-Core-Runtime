// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Linq;
using Godot;

namespace VRBuilder.Core.Utils
{
    /// <summary>
    /// Shared relative-path conversions for Godot.
    /// </summary>
    /// <remarks>
    /// Currently, only audio-based resources are supported there might.
    /// </remarks>
    public static class ResourcePathHelper
    {
        /// <summary>Probed native audio extensions.</summary>
        public static readonly string[] AudioExtensions = [".wav", ".mp3", ".ogg"];

        /// <summary>
        /// Converts a Godot path (res://...) to a relative path without extension.
        /// Returns "" on empty input or outside-Resources (PushError on outside).
        /// </summary>
        public static string NormalizePath(string godotPath)
        {
            if (string.IsNullOrEmpty(godotPath))
            {
                return "";
            }

            var normalized = godotPath.Replace("\\", "/");
            const string marker = "/Resources/";
            var index = normalized.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                GD.PushError("The object is not in the path of a 'Resources' folder.");
                return "";
            }

            if (index + marker.Length >= normalized.Length)
            {
                return "";
            }

            var relative = normalized.Substring(index + marker.Length);

            var dot = relative.LastIndexOf('.');
            return dot >= 0 ? relative.Remove(dot) : relative;
        }

        /// <summary>
        /// Expands a relative audio path to a full res:// path, probing .wav/.mp3/.ogg.
        /// Returns "" when empty or unresolvable.
        /// </summary>
        public static string ExpandAudioPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return "";
            }

            var clean = relativePath.Replace("\\", "/").Trim();
            if (clean.Length == 0)
            {
                return "";
            }

            if (clean.StartsWith("res://", StringComparison.Ordinal) || clean.StartsWith("user://", StringComparison.Ordinal))
            {
                if (ResourceLoader.Exists(clean))
                {
                    return clean;
                }
                if (HasAudioExtension(clean))
                {
                    return "";
                }
                foreach (var ext in AudioExtensions)
                {
                    var probed = clean + ext;
                    if (ResourceLoader.Exists(probed))
                    {
                        return probed;
                    }
                }
                return "";
            }

            var rel = clean.TrimStart('/');
            if (rel.StartsWith("Resources/", StringComparison.Ordinal))
            {
                rel = rel.Substring(10);
            }

            var basePath = "res://Resources/" + rel;
            if (HasAudioExtension(basePath))
            {
                return ResourceLoader.Exists(basePath) ? basePath : "";
            }

            foreach (var ext in AudioExtensions)
            {
                var candidate = basePath + ext;
                if (ResourceLoader.Exists(candidate))
                {
                    return candidate;
                }
            }

            return "";
        }

        /// <summary>
        /// Loads the native AudioStream for a relative path. Null when unresolvable. No PCM transcode.
        /// </summary>
        public static AudioStream? LoadAudioStream(string relativePath)
        {
            var full = ExpandAudioPath(relativePath);
            if (string.IsNullOrEmpty(full))
            {
                return null;
            }
            try
            {
                return ResourceLoader.Load<AudioStream>(full);
            }
            catch
            {
                return null;
            }
        }

        private static bool HasAudioExtension(string path)
        {
            return AudioExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
