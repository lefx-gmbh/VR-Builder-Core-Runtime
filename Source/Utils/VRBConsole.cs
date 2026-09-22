using System;
using System.Collections.Generic;
using VRBuilder.Core.Configuration;
using VRBuilder.Core.Runtime.Registry;
using VRBuilder.UI.Console;

namespace VRBuilder.Core.Utils
{
    /// <summary>
    /// Log messages to an in-world console set up by the runtime configurator.
    /// </summary>
    public static class VRBConsole
    {
        private static ILogConsole console;
        private static Queue<Action> executionQueue = new Queue<Action>();

        //TODO: make a VRBConsoleService or put it into RuntimeService
        // public virtual string VRBConsolePrefab => "Prefabs/DefaultVRBConsole";
        //
        // protected ILogConsole logConsole;
        //
        // public ILogConsole VRBConsole
        // {
        //     get
        //     {
        //         if (logConsole == null)
        //         {
        //             GameObject logConsoleObj = GameObject.Instantiate(Resources.Load<GameObject>(VRBConsolePrefab));
        //             logConsole = logConsoleObj.GetComponent<ILogConsole>();
        //         }
        //
        //         if (logConsole == null)
        //         {
        //             throw new NullReferenceException("Failed to load world console prefab.");
        //         }
        //
        //         return logConsole;
        //     }
        // }
        private static ILogConsole Console
        {
            get
            {
                if (console == null && ServiceRegistry.Has<IRuntimeService>())
                {
                    try
                    {
                        // console = RuntimeConfigurator.Configuration.VRBConsole;
                    }
                    catch (Exception ex)
                    {
                        ForwardingLogger.LogWarning($"Could not initialize VR console: {ex.Message}");
                    }
                }

                return console;
            }
        }

        /// <summary>
        /// Processes the queued log messages thus showing them on the console.
        /// </summary>
        public static void Refresh()
        {
            if (console is null)
            {
                return;
            }

            lock (executionQueue)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue()?.Invoke();
                }
            }
        }

        /// <summary>
        /// Logs a message in the console.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="details">Additional details to show when expanding the message.</param>
        /// <param name="show">If true, show the console when the message is logged.</param>
        public static void Log(string message, string details = "", bool show = false)
        {
            Enqueue(message, details, LogSeverity.Log, show);
        }

        /// <summary>
        /// Logs a warning in the console.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="details">Additional details to show when expanding the message.</param>
        /// <param name="show">If true, show the console when the message is logged.</param>
        public static void LogWarning(string message, string details = "", bool show = false)
        {
            Enqueue(message, details, LogSeverity.Warning, show);
        }

        /// <summary>
        /// Logs an error in the console.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="details">Additional details to show when expanding the message.</param>
        /// <param name="show">If true, show the console when the message is logged.</param>
        public static void LogError(string message, string details = "", bool show = true)
        {
            Enqueue(message, details, LogSeverity.Error, show);
        }

        /// <summary>
        /// Logs an exception in the console.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="show">If true, show the console when the message is logged.</param>
        public static void LogException(Exception ex, bool show = true)
        {
            Enqueue(ex.Message, ex.StackTrace, LogSeverity.Exception, show);
        }

        /// <summary>
        /// Clears all messages from the console.
        /// </summary>
        public static void Clear()
        {
            ILogConsole target = Console;

            if (target == null)
            {
                return;
            }

            lock (executionQueue)
            {
                executionQueue.Enqueue(() => console.Clear());
            }

            target.SetDirty();
        }

        /// <summary>
        /// Toggles the console between visible and hidden.
        /// </summary>
        public static void Toggle()
        {
            ILogConsole target = Console;

            if (target == null)
            {
                return;
            }

            lock (executionQueue)
            {
                executionQueue.Enqueue(() => console.Toggle());
            }

            target.SetDirty();
        }

        private static void Enqueue(string message, string details, LogSeverity severity, bool show)
        {
            lock (executionQueue)
            {
                executionQueue.Enqueue(() =>
                {
                    console.LogMessage(message, details, severity);

                    if (show)
                    {
                        console.Show();
                    }
                });
            }

            Console?.SetDirty();
        }
    }
}