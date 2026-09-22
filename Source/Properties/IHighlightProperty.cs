// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using System;
using VRBuilder.Core.Primitives;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Interface for scene objects that can be highlighted.
    /// </summary>
    public interface IHighlightProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Is object currently highlighted.
        /// </summary>
        bool IsHighlighted { get; }

        /// <summary>
        /// Emitted when the object gets highlighted.
        /// </summary>
        event Action<IHighlightPropertyEventArgs> HighlightStartedAction;

        /// <summary>
        /// Emitted when the object gets unhighlighted.
        /// </summary>
        event Action<IHighlightPropertyEventArgs> HighlightEndedAction;

        /// <summary>
        /// Highlight this object and use <paramref name="highlightColor"/>.
        /// </summary>
        /// <param name="highlightColor">Color to use for highlighting.</param>
        void Highlight(IColor highlightColor);

        /// <summary>
        /// Disable highlight.
        /// </summary>
        void Unhighlight();
    }

    /// <summary>
    /// Event arguments for highlight events.
    /// </summary>
    public interface IHighlightPropertyEventArgs
    {
    }
}