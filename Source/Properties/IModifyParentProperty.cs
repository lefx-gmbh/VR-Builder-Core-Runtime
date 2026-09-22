// Copyright (c) 2026 Aron Schaub
// SPDX-License-Identifier: Apache-2.0

using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Interface for reparenting scene objects in the hierarchy.
    /// </summary>
    /// <remarks>
    /// This Property makes it most obvious, how different TinkerFlow and VR-Builder work depending on if we are in Unity or Godot.
    ///
    /// <b>Unity Model</b>
    ///
    /// Unity can have multiple Components on a GameObject. therefore we can very simply access the parent.
    /// 
    /// GameObject "DoorFrame"                 
    ///  └── GameObject "Door"
    ///      ├── ProcessSceneObject (component)
    ///      ├── ProcessSceneObjectProperty: ModifyParentProperty (component)
    ///      ├── MeshRenderer
    ///      └── Collider
    ///
    ///   → GetComponent&lt;ProcessSceneObject&gt;()   ← sibling-to-sibling lookup
    ///   → ModifyParentProperty lives on the SAME GameObject as ProcessSceneObject
    ///
    /// <b>Godot Model</b>
    ///
    /// In Godot only one Script can be attached to a Node. Therefore we decided to make our SceenObjects children nodes of the ISceneObject.
    /// This means that we have to get "one higher" on the Nodes to get the Parent.
    /// 
    /// Node3D "DoorFrame"                  ← the actual parent in the scene tree
    ///   └── Node3D "Door"                 ← the ISceneObject itself
    ///         ├── Node3D "ModifyParentProperty"   ← "component" as child node
    ///         ├── Node3D "SomeOtherProperty"
    ///         └── CollisionShape3D
    ///
    ///   → GetParent()                        → "Door"    (the ISceneObject)
    ///   → GetParent().GetParent()            → "DoorFrame" (the actual target parent)
    ///
    /// <b>The chain in code</b>
    /// this.GetParent().GetParent().RemoveChild(GetParent())
    /// │        │            │              │         └─ "Door" SceneObject again
    /// │        │            │              └─ removes it from...
    /// │        │            └─ "DoorFrame" (the grandparent / real parent in scene tree)
    /// │        └─ "Door" SceneObject
    /// └─ "ModifyParentProperty" (this property node)
    /// </remarks>
    public interface IModifyParentProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Detaches the scene object from its current parent.
        /// </summary>
        void UnsetParent();

        /// <summary>
        /// Attaches the scene object to <paramref name="parentObject"/> as a child.
        /// </summary>
        /// <param name="parentObject">The new parent scene object.</param>
        /// <param name="snapToParentTransform">If true, the object's world position and rotation snap to the parent's transform before or during reparenting.</param>
        void SetParent(ISceneObject parentObject, bool snapToParentTransform);
    }
}