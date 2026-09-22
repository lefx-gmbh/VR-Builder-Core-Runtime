// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.Registry;

namespace VRBuilder.Core.ProcessRunning
{
    /// <summary>
    /// Service that drives a process: <see cref="Initialize"/> sets the process up, <see cref="Update"/>
    /// advances it every frame, and <see cref="Events"/> exposes its lifecycle. Resolve it via the
    /// <see cref="VRBuilder.Core.Runtime.Registry.ServiceRegistry"/>; the default implementation is
    /// <see cref="DefaultProcessRunner"/>.
    /// </summary>
    public interface IProcessRunner : IService<IProcessRunnerConfiguration>
    {
        /// <summary>
        /// The currently running process, or <c>null</c> if none is running.
        /// </summary>
        IProcess? CurrentProcess { get; }

        /// <summary>
        /// The current Chapter, or <c>null</c> if none is running.
        /// </summary>
        IChapter? CurrentChapter { get; }

        /// <summary>
        /// The current step of the running process, or <c>null</c> if none is running.
        /// </summary>
        IStep? CurrentStep { get; }

        /// <summary>
        /// <c>true</c> if a process has been initialized and is currently active.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Lifecycle events for the current process.
        /// These mirror the events on <see cref="ProcessEvents"/>.
        /// </summary>
        ProcessEvents Events { get; }

        /// <summary>
        /// Initializes the runner with a process, creating required scene components.
        /// </summary>
        /// <param name="process">The process to run.</param>
        void Initialize(IProcess process);

        /// <summary>
        /// Starts execution of the initialized process.
        /// </summary>
        void Start();

        /// <summary>
        /// Advances the currently running process and raises the corresponding lifecycle events.
        /// </summary>
        void Update();

        /// <summary>
        /// Stops the running process and releases runner state.
        /// </summary>
        void Stop();

        /// <summary>
        /// Sets the specified chapter as the next chapter to execute.
        /// </summary>
        void SetNextChapter(IChapter chapter);

        /// <summary>
        /// Skips the current step using the given transition to fast-forward.
        /// </summary>
        void SkipStep(ITransition transition);

        /// <summary>
        /// Skips the given number of chapters ahead.
        /// </summary>
        void SkipChapters(int numberOfChapters);

        /// <summary>
        /// Skips the current chapter entirely.
        /// </summary>
        void SkipCurrentChapter();

        /// <summary>
        /// Notifies the runner that the given scene was unloaded so it can reset its state.
        /// </summary>
        /// <param name="sceneName">The name of the unloaded scene.</param>
        void OnSceneUnloaded(string sceneName);
    }
}