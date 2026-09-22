namespace VRBuilder.Core.Utils.Logging
{
    /// <summary>
    /// Configures which lifecycle events are forwarded to the logger.
    /// </summary>
    public interface ILifeCycleLoggingConfiguration
    {
        /// <summary>
        /// <c>true</c> to log behavior lifecycle events.
        /// </summary>
        bool LogBehaviors { get; }

        /// <summary>
        /// <c>true</c> to log condition lifecycle events.
        /// </summary>
        bool LogConditions { get; }

        /// <summary>
        /// <c>true</c> to log chapter lifecycle events.
        /// </summary>
        bool LogChapters { get; }

        /// <summary>
        /// <c>true</c> to log step lifecycle events.
        /// </summary>
        bool LogSteps { get; }

        /// <summary>
        /// <c>true</c> to log transition lifecycle events.
        /// </summary>
        bool LogTransitions { get; }

        /// <summary>
        /// <c>true</c> to log data property change events.
        /// </summary>
        bool LogDataPropertyChanges { get; }

        /// <summary>
        /// <c>true</c> to log lock state changes.
        /// </summary>
        bool LogLockState { get; }
    }
}