// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;

namespace VRBuilder.Core
{
    /// <summary>
    /// Engine-agnostic logger. Delegates are set by the engine-specific
    /// initializer in Core during startup. No Unity/Godot dependencies.
    /// </summary>
    public static class ForwardingLogger
    {
        /// <summary>
        /// Delegate used by <see cref="Log(object)"/>.
        /// </summary>
        public static Action<object> LogAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogFormat(string, object[])"/>.
        /// </summary>
        public static Action<string, object[]> LogFormatAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogWarning(object)"/>.
        /// </summary>
        public static Action<object> LogWarningAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogWarningFormat(string, object[])"/>.
        /// </summary>
        public static Action<string, object[]> LogWarningFormatAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogError(object)"/>.
        /// </summary>
        public static Action<object> LogErrorAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogErrorFormat(string, object[])"/>.
        /// </summary>
        public static Action<string, object[]> LogErrorFormatAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogAssertion(object)"/>.
        /// </summary>
        public static Action<object> LogAssertionAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogAssertionFormat(string, object[])"/>.
        /// </summary>
        public static Action<string, object[]> LogAssertionFormatAction { get; set; }

        /// <summary>
        /// Delegate used by <see cref="LogException(Exception)"/>.
        /// </summary>
        public static Action<Exception> LogExceptionAction { get; set; }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(object message) => LogAction?.Invoke(message);

        /// <summary>
        /// Logs a formatted message.
        /// </summary>
        /// <param name="format">The composite format string.</param>
        /// <param name="args">The objects to format.</param>
        public static void LogFormat(string format, params object[] args) => LogFormatAction?.Invoke(format, args);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(object message) => LogWarningAction?.Invoke(message);

        /// <summary>
        /// Logs a formatted warning message.
        /// </summary>
        /// <param name="format">The composite format string.</param>
        /// <param name="args">The objects to format.</param>
        public static void LogWarningFormat(string format, params object[] args) => LogWarningFormatAction?.Invoke(format, args);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogError(object message) => LogErrorAction?.Invoke(message);

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        /// <param name="format">The composite format string.</param>
        /// <param name="args">The objects to format.</param>
        public static void LogErrorFormat(string format, params object[] args) => LogErrorFormatAction?.Invoke(format, args);

        /// <summary>
        /// Logs an assertion failure message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogAssertion(object message) => LogAssertionAction?.Invoke(message);

        /// <summary>
        /// Logs a formatted assertion failure message.
        /// </summary>
        /// <param name="format">The composite format string.</param>
        /// <param name="args">The objects to format.</param>
        public static void LogAssertionFormat(string format, params object[] args) => LogAssertionFormatAction?.Invoke(format, args);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public static void LogException(Exception exception) => LogExceptionAction?.Invoke(exception);
    }
}