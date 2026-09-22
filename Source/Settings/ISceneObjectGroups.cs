using System;
using System.Collections.Generic;

namespace VRBuilder.Core.Settings
{
    /// <summary>
    /// Manages named groups of scene objects, identified by a <see cref="Guid"/>.
    /// </summary>
    public interface ISceneObjectGroups
    {
        /// <summary>
        /// Returns the display label of the group identified by the given tag.
        /// </summary>
        /// <param name="guid">The tag identifying the group.</param>
        /// <returns>The display label of the group.</returns>
        string GetLabel(Guid guid);

        /// <summary>
        /// Returns <c>true</c> if a group with the given tag exists.
        /// </summary>
        /// <param name="guid">The tag identifying the group.</param>
        /// <returns><c>true</c> if the group exists, otherwise <c>false</c>.</returns>
        bool GroupExists(Guid guid);

        /// <summary>
        /// Returns <c>true</c> if any of the given tags belongs to an existing group.
        /// </summary>
        /// <param name="guids">The tags to check.</param>
        /// <returns><c>true</c> if any tag belongs to an existing group, otherwise <c>false</c>.</returns>
        bool ContainsAny(IEnumerable<Guid> guids);

        /// <summary>
        /// Returns <c>true</c> if a group with the given name can be created.
        /// </summary>
        /// <param name="newGroup">The name of the proposed new group.</param>
        /// <returns><c>true</c> if the group can be created, otherwise <c>false</c>.</returns>
        bool CanCreateGroup(string newGroup);

        /// <summary>
        /// Raised when the set of groups changes.
        /// </summary>
        event Action Changed;
    }
}