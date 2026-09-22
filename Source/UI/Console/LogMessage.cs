namespace VRBuilder.UI.Console
{
    /// <summary>
    /// A message logged in an <see cref="ILogConsole"/>.
    /// </summary>
    public struct LogMessage
    {
        /// <summary>
        /// The main message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Additional information provided in the message.
        /// </summary>
        public string Details { get; private set; }

        /// <summary>
        /// Severity of the message logged.
        /// </summary>
        public LogSeverity Severity { get; private set; }

        /// <summary>
        /// Creates a new log message with the given content and severity.
        /// </summary>
        /// <param name="message">The main message.</param>
        /// <param name="details">Additional information provided with the message.</param>
        /// <param name="severity">The severity of the message.</param>
        public LogMessage(string message, string details, LogSeverity severity)
        {
            Message = message;
            Details = details;
            Severity = severity;
        }
    }
}