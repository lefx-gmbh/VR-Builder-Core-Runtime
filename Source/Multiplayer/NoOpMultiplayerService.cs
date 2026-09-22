using System;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Multiplayer
{
    /// <summary>
    /// A <see cref="IMultiplayerService"/> implementation that performs no network synchronization and
    /// grants authority to the requesting caller immediately.
    /// </summary>
    public class NoOpMultiplayerService : IMultiplayerService
    {
        /// <inheritdoc/>
        public void RequestAuthority(ISceneObject sceneObject, Action<ISceneObject> onAuthorityGranted = null)
        {
            onAuthorityGranted?.Invoke(sceneObject);
        }
    }
}