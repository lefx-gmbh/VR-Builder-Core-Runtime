using System;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Multiplayer
{
    /// <summary>
    /// Provides the ability to request authority over scene objects in a multiplayer session.
    /// </summary>
    public interface IMultiplayerService
    {
        /// <summary>
        /// Requests authority on the specified scene object.
        /// </summary>
        public void RequestAuthority(ISceneObject sceneObject, Action<ISceneObject> onAuthorityGranted = null);
    }
}