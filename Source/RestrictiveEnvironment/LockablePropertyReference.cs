// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System.Runtime.Serialization;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.Core.RestrictiveEnvironment
{
    /// <summary>
    /// Serializable reference to a <see cref="LockableProperty"/>
    /// </summary>
    [DataContract(IsReference = true)]
    public class LockablePropertyReference
    {
        /// <summary>
        /// Reference to the scene object the LockableProperty is attached to.
        /// </summary>
        [DataMember]
        public SingleSceneObjectReference TargetObject;

        /// <summary>
        /// Type name of the LockableProperty.
        /// </summary>
        [DataMember]
        public string Type;

        [IgnoreDataMember]
        private ILockableProperty property;

        /// <summary>
        /// Initializes an empty <see cref="LockablePropertyReference"/> that is resolved lazily on first <see cref="GetProperty"/> call.
        /// </summary>
        public LockablePropertyReference()
        {
        }

        /// <summary>
        /// Initializes a <see cref="LockablePropertyReference"/> that points to the given property's scene object and type.
        /// </summary>
        /// <param name="property">The lockable property to reference; must not be <c>null</c>.</param>
        public LockablePropertyReference(ILockableProperty property)
        {
            TargetObject = new SingleSceneObjectReference(property.SceneObject.Guid);
            Type = property.GetType().AssemblyQualifiedName;
        }

        /// <summary>
        /// Returns the referenced <see cref="LockableProperty"/>.
        /// </summary>
        public ILockableProperty GetProperty()
        {
            if (property == null)
            {
                foreach (ISceneObjectProperty prop in TargetObject.Value.Properties)
                {
                    if (prop.GetType().AssemblyQualifiedName.Equals(Type))
                    {
                        property = (ILockableProperty)prop;
                        break;
                    }
                }
            }

            return property;
        }
    }
}