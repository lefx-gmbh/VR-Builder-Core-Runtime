// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.Properties
{
    /// <summary>
    /// Base contract for a property attached to a <see cref="ISceneObject"/> (e.g. lockable, scalable,
    /// audio or highlight properties). In Godot, a property is modeled as a child node of the
    /// <c>ProcessSceneObject</c>; <see cref="SceneObject"/> points back to the owning object.
    /// Implement the concrete property interfaces (e.g. <c>IScaleProperty</c>) rather than this directly.
    /// </summary>
    public interface ISceneObjectProperty
    {
        /// <summary>
        /// The scene object this property belongs to.
        /// </summary>
        ISceneObject SceneObject { get; }
    }
}