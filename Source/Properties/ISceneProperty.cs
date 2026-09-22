// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Marks a scene object property that can load a scene at runtime, either asynchronously
    /// via <see cref="StartLoadAsync"/> or synchronously via <see cref="LoadSynchronously"/>.
    /// Inherits <see cref="ISceneObjectProperty"/>.
    /// </summary>
    public interface ISceneProperty : ISceneObjectProperty
    {
        /// <summary>Starts an asynchronous scene load and returns a callback to poll its completion.</summary>
        /// <param name="scenePath">Path identifying the scene to load.</param>
        /// <param name="loadAdditively">When <c>true</c>, the scene is loaded additively on top of the current one; otherwise the current scene is replaced.</param>
        /// <returns>An <see cref="IAsyncCallback"/> whose <see cref="IAsyncCallback.isDone"/> reflects whether the load has finished.</returns>
        IAsyncCallback StartLoadAsync(string scenePath, bool loadAdditively);

        /// <summary>Loads the scene synchronously, blocking until loading completes.</summary>
        /// <param name="scenePath">Path identifying the scene to load.</param>
        /// <param name="loadAdditively">When <c>true</c>, the scene is loaded additively on top of the current one; otherwise the current scene is replaced.</param>
        void LoadSynchronously(string scenePath, bool loadAdditively);
    }

    /// <summary>Callback used to poll the completion of an asynchronous scene load.</summary>
    public interface IAsyncCallback
    {
        /// <summary>Gets whether the underlying asynchronous operation has completed. <c>true</c> when done; otherwise <c>false</c>.</summary>
        bool isDone { get; }
    }
}