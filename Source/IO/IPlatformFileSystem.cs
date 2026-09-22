// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VRBuilder.Core.Serialization;

namespace VRBuilder.Core.IO
{
    /// <summary>
    /// Interface with basic platform operations for reading and saving files in Unity.
    /// </summary>
    /// <remarks>Operations are done for the StreamingAssets and platform persistent data folders.</remarks>
    public interface IPlatformFileSystem
    {
        /// <summary>
        /// The path to the platform's StreamingAssets folder (Read Only).
        /// </summary>
        public string StreamingAssetsPath { get; }

        /// <summary>
        /// The path to the platform's persistent data directory (Read Only).
        /// </summary>
        public string PersistentDataPath { get; }
        
        /// <summary>
        /// Loads a file stored at <paramref name="filePath"/>.
        /// </summary>
        /// <remarks><paramref name="filePath"/> must be relative to the StreamingAssets or the persistent data folder.</remarks>
        /// <returns>The contents of the file into a byte array.</returns>
        /// <exception cref="FileNotFoundException">Exception thrown if the file does not exist.</exception>
        public Task<byte[]> Read(string filePath);

        /// <summary>
        /// Loads a file stored at <paramref name="filePath"/>.
        /// </summary>
        /// <remarks><paramref name="filePath"/> must be relative to the StreamingAssets or the persistent data folder.</remarks>
        /// <returns>Returns a `string` with the content of the file.</returns>
        /// <exception cref="FileNotFoundException">Exception thrown if the file does not exist.</exception>
        public Task<string> ReadAllText(string filePath);

        /// <summary>
        /// Saves given <paramref name="fileData"/> in provided <paramref name="filePath"/>.
        /// </summary>
        /// <remarks><paramref name="filePath"/> must be relative to <see cref="PersistentDataPath"/>.</remarks>
        /// <returns>Returns true if <paramref name="fileData"/> could be saved successfully; otherwise, false.</returns>
        public Task<bool> Write(string filePath, byte[] fileData);

        /// <summary>
        /// Returns true if given <paramref name="filePath"/> contains the name of an existing file under the StreamingAssets or platform persistent data folder; otherwise, false.
        /// </summary>
        /// <remarks><paramref name="filePath"/> must be relative to the StreamingAssets or the platform persistent data folder.</remarks>
        public Task<bool> Exists(string filePath);

        /// <summary>
        /// Returns the names of files (including their paths) that match the specified search pattern in the specified directory relative to the Streaming Assets folder.
        /// </summary>
        /// <param name="path">The relative path to the Streaming Assets folder. This string is not case-sensitive.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in <paramref name="path" />.
        /// Depending on the platform, this parameter can contain a combination of valid literal path and wildcard (* and ?) characters (see implementations of <see cref="IPlatformFileSystem"/>), but doesn't support regular expressions.
        /// </param>
        public IEnumerable<string> FetchStreamingAssetsFilesAt(string path, string searchPattern);

        /// <summary>
        /// Fetch the manifest file of the specific file platform.
        /// </summary>
        /// <param name="processName">Name of the process.</param>
        /// <param name="manifestPath">Path to the manifest.</param>
        /// <param name="serializer">Serializer of the manifest file.</param>
        /// <returns></returns>
        public Task<IProcessAssetManifest> FetchManifest(string processName, string manifestPath, IProcessSerializer serializer);
    }
}
