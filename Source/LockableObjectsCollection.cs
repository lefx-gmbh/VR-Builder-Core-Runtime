// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using VRBuilder.Core.Properties;
using VRBuilder.Core.RestrictiveEnvironment;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Core.Utils;

namespace VRBuilder.Core
{
    /// <summary>
    /// Collection of <see cref="ISceneObject"/>s that can be locked and unlocked during a step.
    /// Additionally, checks if objects are automatically or manually unlocked.
    /// </summary>
    public class LockableObjectsCollection
    {
        private Step.EntityData data;
        private List<LockablePropertyData> toUnlock;

        /// <summary>
        /// Creates a collection of lockable scene objects for the given step data.
        /// </summary>
        /// <param name="entityData">The step data whose lock configuration is used to build the collection.</param>
        public LockableObjectsCollection(Step.EntityData entityData)
        {
            toUnlock = PropertyReflectionHelper.ExtractLockablePropertiesFromStep(entityData).ToList();
            data = entityData;

            CreateSceneObjects();
        }

        /// <summary>
        /// Returns the current tags to manually unlock.
        /// </summary>
        public IEnumerable<Guid> TagsToUnlock => data.GroupsToUnlock.Keys;

        /// <summary>
        /// All scene objects currently managed by this collection.
        /// </summary>
        public List<ISceneObject> SceneObjects { get; set; } = new List<ISceneObject>();

        private void CreateSceneObjects()
        {
            CleanProperties();

            if (data.ToUnlock.Any(propertyReference => propertyReference.TargetObject.Value == null))
            {
                data.ToUnlock = data.ToUnlock.Where(propertyReference => propertyReference.TargetObject.Value != null).ToList();
                ForwardingLogger.LogWarning($"Null references have been found and removed in the manually unlocked objects of step '{data.Name}'.\n" +
                                            $"Did you delete or reset any Process Scene Objects?");
            }

            foreach (LockablePropertyReference propertyReference in data.ToUnlock)
            {
                AddSceneObject(propertyReference.TargetObject.Value);
            }

            foreach (LockablePropertyData propertyData in toUnlock)
            {
                AddSceneObject(propertyData.Property.SceneObject);
            }
        }

        /// <summary>
        /// Adds the scene object to the collection if it is not already present.
        /// </summary>
        /// <param name="sceneObject">The scene object to add.</param>
        public void AddSceneObject(ISceneObject sceneObject)
        {
            if (SceneObjects.Contains(sceneObject) == false)
            {
                SceneObjects.Add(sceneObject);
                SortSceneObjectList();
            }
        }

        private void SortSceneObjectList()
        {
            SceneObjects.Sort((obj1, obj2) => string.Compare(obj1.ToString(), obj2.ToString(), StringComparison.Ordinal));
        }

        /// <summary>
        /// Removes the scene object from the collection and removes its references from the manual unlock list.
        /// </summary>
        /// <param name="sceneObject">The scene object to remove.</param>
        public void RemoveSceneObject(ISceneObject sceneObject)
        {
            if (SceneObjects.Remove(sceneObject))
            {
                data.ToUnlock = data.ToUnlock.Where(property =>
                {
                    if (property.GetProperty() == null)
                    {
                        return false;
                    }

                    return property.GetProperty().SceneObject != sceneObject;
                }).ToList();
            }
        }

        /// <summary>
        /// Returns true if the given property is in the manual unlock list.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns><c>true</c> if the property is manually unlocked, otherwise <c>false</c>.</returns>
        public bool IsInManualUnlockList(ILockableProperty property)
        {
            foreach (LockablePropertyReference lockableProperty in data.ToUnlock)
            {
                if (property == lockableProperty.GetProperty())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if any property of the scene object is automatically unlocked.
        /// </summary>
        /// <param name="sceneObject">The scene object to check.</param>
        /// <returns><c>true</c> if the object is automatically unlocked, otherwise <c>false</c>.</returns>
        public bool IsUsedInAutoUnlock(ISceneObject sceneObject)
        {
            return toUnlock.Any(propertyData => propertyData.Property.SceneObject == sceneObject);
        }

        /// <summary>
        /// Returns true if the given property is in the automatic unlock list.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns><c>true</c> if the property is automatically unlocked, otherwise <c>false</c>.</returns>
        public bool IsInAutoUnlockList(ILockableProperty property)
        {
            foreach (LockablePropertyData lockableProperty in toUnlock)
            {
                if (property == lockableProperty.Property)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the property from the manual unlock list.
        /// </summary>
        /// <param name="property">The property to remove.</param>
        public void Remove(ILockableProperty property)
        {
            data.ToUnlock = data.ToUnlock.Where(reference => reference.GetProperty() != property).ToList();
        }

        /// <summary>
        /// Adds the property to the manual unlock list.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void Add(ILockableProperty property)
        {
            data.ToUnlock = data.ToUnlock.Union(new[] { new LockablePropertyReference(property), }).ToList();
        }

        /// <summary>
        /// Adds a group for the given tag to the groups to unlock.
        /// </summary>
        /// <param name="tag">The tag identifying the group.</param>
        public void AddGroup(Guid tag)
        {
            if (data.GroupsToUnlock.ContainsKey(tag))
            {
                return;
            }

            data.GroupsToUnlock.Add(tag, new List<Type>());
        }

        /// <summary>
        /// Removes the group for the given tag from the groups to unlock.
        /// </summary>
        /// <param name="tag">The tag identifying the group.</param>
        public void RemoveGroup(Guid tag)
        {
            data.GroupsToUnlock.Remove(tag);
        }

        /// <summary>
        /// Adds the property type to the group identified by the given tag.
        /// </summary>
        /// <param name="tag">The tag identifying the group.</param>
        /// <param name="property">The property type to add.</param>
        public void AddPropertyToGroup(Guid tag, Type property)
        {
            if (data.GroupsToUnlock.ContainsKey(tag) == false)
            {
                return;
            }

            data.GroupsToUnlock[tag] = data.GroupsToUnlock[tag].Union(new[] { property }).ToList();
        }

        /// <summary>
        /// Removes the property type from the group identified by the given tag.
        /// </summary>
        /// <param name="tag">The tag identifying the group.</param>
        /// <param name="property">The property type to remove.</param>
        public void RemovePropertyFromGroup(Guid tag, Type property)
        {
            if (data.GroupsToUnlock.ContainsKey(tag) == false)
            {
                return;
            }

            data.GroupsToUnlock[tag] = data.GroupsToUnlock[tag].Where(p => p != property).ToList();
        }

        /// <summary>
        /// Returns true if the property type is enabled for the group identified by the given tag.
        /// </summary>
        /// <param name="tag">The tag identifying the group.</param>
        /// <param name="property">The property type to check.</param>
        /// <returns><c>true</c> if the property is enabled for the group, otherwise <c>false</c>.</returns>
        public bool IsPropertyEnabledForGroup(Guid tag, Type property)
        {
            return data.GroupsToUnlock.ContainsKey(tag) && data.GroupsToUnlock[tag].Contains(property);
        }

        private void CleanProperties()
        {
            data.ToUnlock = data.ToUnlock.Where(reference => reference.TargetObject != null && reference.TargetObject.IsEmpty() == false).ToList();
        }
    }
}