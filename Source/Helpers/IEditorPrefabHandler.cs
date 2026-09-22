using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Helpers
{
    /// <summary>
    /// Defines a handler that is invoked when a duplicate GUID is detected during scene object registration
    /// in <see cref="SceneObjectRegistry"/>.
    /// </summary>
    /// <remarks>
    /// When <see cref="SceneObjectRegistry"/> detects that an <see cref="ISceneObject"/> being registered
    /// carries a GUID that already exists in the registry, it assigns a new GUID via
    /// <see cref="ISceneObject.SetObjectId"/> and then calls this handler to allow the editor-specific layer
    /// to persist the reassigned GUID (e.g., by marking the prefab as dirty or recording property
    /// modifications in the Unity editor).
    ///
    /// The handler is injected through <see cref="ISceneObjectRegistryConfiguration.EditorPrefabHandler"/>
    /// and follows the strategy pattern so that engine-specific logic (Unity vs. Godot) can be provided
    /// without the engine-agnostic core knowing about it.
    /// This is Most likely a no-op in Godot.
    /// </remarks>
    public interface IEditorPrefabHandler
    {
        /// <summary>
        /// called when a duplicate GUID is detected in the <see cref="ISceneObjectRegistry.Register"/>
        /// </summary>
        /// <param name="sceneObject">the duplicated (new) SceneObject</param>
        void OnDuplicateGuidDetected(ISceneObject sceneObject);
    }
}