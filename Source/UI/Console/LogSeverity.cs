namespace VRBuilder.UI.Console
{
    /// <summary>
    /// Severity of a message logged in an <see cref="ILogConsole"/>. Mirrors the engine log
    /// levels without depending on UnityEngine, keeping the console contract pure C#.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// A regular log message.
        /// </summary>
        Log,

        /// <summary>
        /// A warning message.
        /// </summary>
        Warning,

        /// <summary>
        /// An error message.
        /// </summary>
        Error,

        /// <summary>
        /// An exception message.
        /// </summary>
        Exception,

        /// <summary>
        /// An assertion failure message.
        /// </summary>
        Assert
    }
}