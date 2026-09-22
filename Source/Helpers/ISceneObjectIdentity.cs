// Modifications copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Helpers
{
    /// <summary>
    /// Provides a per-instance identity for an <see cref="ISceneObject"/> that distinguishes it from
    /// other objects that share the same serialized <see cref="ISceneObject.Guid"/>.
    /// </summary>
    /// <remarks>
    /// An <see cref="ISceneObject"/> has two separate identity concepts:
    /// <list type="bullet">
    ///   <item><description>
    ///     <b>GUID</b> (<see cref="ISceneObject.Guid"/>) — the serialized identity stored in scene or
    ///     prefab files. It persists across saves and reloads and is the key used in
    ///     <see cref="SceneObjects.SceneObjectRegistry"/>.
    ///   </description></item>
    ///   <item><description>
    ///     <b>Instance identity</b> (this interface) — a transient engine-native identifier
    ///     (<c>GetInstanceID</c> in Unity, <c>GetInstanceId</c> in Godot) that is unique per
    ///     live GameObject or node. It differentiates multiple in-scene instances that initially
    ///     carry the same serialized GUID (for example, several copies of the same prefab).
    ///   </description></item>
    /// </list>
    ///
    /// The identity is used by <see cref="SceneObjects.SceneObjectRegistry.HasDuplicateGuid"/> to
    /// decide whether a GUID collision is a true duplicate (different instances) or a re-registration
    /// of the same instance. When a duplicate is detected, the registry assigns a new GUID
    /// via <see cref="ISceneObject.SetObjectId"/> and notifies the
    /// <see cref="IEditorPrefabHandler"/>.
    ///
    /// The implementation is injected through
    /// <see cref="SceneObjects.ISceneObjectRegistryConfiguration.SceneObjectIdentity"/>.
    /// </remarks>
    public interface ISceneObjectIdentity
    {
        /// <summary>
        /// Returns a unique numeric identifier for the specified <paramref name="obj"/> that
        /// distinguishes it from other in-scene instances.
        /// </summary>
        /// <param name="obj">The scene object to identify.</param>
        /// <returns>
        /// An engine-native instance ID unique to this live object
        /// (<c>GetInstanceID</c> in Unity, <c>GetInstanceId</c> in Godot).
        /// Returns <c>0</c> if the object cannot be resolved to an engine-specific instance.
        /// this could admittedly cause shadowing in edge casing 
        /// </returns>
        ulong GetIdentity(ISceneObject obj);
    }
}