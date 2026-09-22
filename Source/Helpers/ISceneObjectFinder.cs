using System.Collections.Generic;

namespace VRBuilder.Core.Helpers
{
    /// <summary>
    /// Provides a way to discover all scene objects without coupling the engine-agnostic core to
    /// Unity or Godot APIs.
    /// </summary>
    /// <remarks>
    /// The engine-agnostic <see cref="SceneObjects.SceneObjectRegistry"/> cannot reference Unity's
    /// <c>Object.FindObjectsByType</c> or Godot's scene tree traversal directly. This interface
    /// abstracts that lookup so each engine provides its own implementation.
    ///
    /// <b>Unity</b> — uses <c>SceneUtils.GetActiveAndInactiveComponents&lt;ProcessSceneObject&gt;</c>
    /// to find every <c>ProcessSceneObject</c> across all loaded scenes.<br/>
    /// <b>Godot</b> — traverses the <c>SceneManager</c> node tree via <c>GetChildren(true)</c> and
    /// filters by type.
    ///
    /// The finder is injected through <see cref="SceneObjects.ISceneObjectRegistryConfiguration.SceneObjectFinder"/>
    /// and is called from <see cref="SceneObjects.SceneObjectRegistry.RegisterAll"/>.
    /// </remarks>
    public interface ISceneObjectFinder
    {
        /// <summary>
        /// Returns all scene objects that implement or derive from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of scene object to find. Must be a reference type.</typeparam>
        /// <returns>All discovered scene objects assignable to <typeparamref name="T"/>.</returns>
        IEnumerable<T> FindAllSceneObjects<T>() where T : class;
    }
}