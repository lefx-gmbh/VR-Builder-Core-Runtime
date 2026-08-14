// Copyright (c) 2013-2019 Innoactive GmbH
// Licensed under the Apache License, Version 2.0
// Modifications copyright (c) 2021-2026 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VRBuilder.Core.Conditions;
using VRBuilder.Core.Properties;
using VRBuilder.Core.RestrictiveEnvironment;
using VRBuilder.Core.Runtime.Registry;
using VRBuilder.Core.SceneObjects;
using VRBuilder.Utils;

namespace VRBuilder.Core.Utils
{
        /// <summary>
        /// Helper class which provides methods to extract <see cref="LockablePropertyData"/> from different process entities.
        /// Uses a convention-based reflection approach so that conditions, transitions, and steps automatically discover
        /// their referenced lockable properties without requiring each entity to manually implement the extraction logic.
        /// </summary>
        public static class PropertyReflectionHelper
        {
        /// <summary>
        /// Engine-specific delegate that resolves required component dependencies for a property type.
        /// Unity sets this to read <c>UnityEngine.RequireComponent</c> attributes.
        /// Godot sets this to read <c>RequireComponentAttribute</c> attributes.
        /// Defaults to empty (no dependencies resolved) if no engine initializer has run.
        /// </summary>
        public static Func<Type, IEnumerable<Type>> ResolveRequiredComponents { get; set; } = _ => Enumerable.Empty<Type>();

        /// <summary>
        /// Engine-specific delegate that determines whether a concrete property type should be excluded
        /// from lockable property resolution. Unity sets this to filter out types whose assemblies
        /// reference <c>UnityEditor</c>, preventing editor-only types from being resolved at runtime.
        /// Defaults to <c>false</c> (exclude nothing) if no engine initializer has run.
        /// </summary>
        public static Func<Type, bool> ShouldExcludeType { get; set; } = _ => false;

        /// <summary>
        /// Walks every transition and condition in the given step and collects all <see cref="LockablePropertyData"/>
        /// that the step references via its conditions.
        /// </summary>
        /// <param name="data">The step data whose transitions and conditions are scanned for lockable property references.
        /// May be null; returns an empty list in that case.</param>
        /// <returns>A list of all <see cref="LockablePropertyData"/> found across all conditions of all transitions.
        /// Never null. Returns an empty list when <paramref name="data"/> is null or contains no conditions.</returns>
        public static List<LockablePropertyData> ExtractLockablePropertiesFromStep(IStepData data)
        {
            List<LockablePropertyData> result = new List<LockablePropertyData>();

            if (data == null)
            {
                return result;
            }

            foreach (ITransition transition in data.Transitions.Data.Transitions)
            {
                foreach (ICondition condition in transition.Data.Conditions)
                {
                    if (condition is ILockablePropertiesProvider lockablePropertiesProvider)
                    {
                        result.AddRange(lockablePropertiesProvider.GetLockableProperties());
                    }
                    else
                    {
                        result.AddRange(ExtractLockablePropertiesFromCondition(condition.Data));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts all <see cref="LockablePropertyData"/> from a single condition's data by reflecting on its
        /// concrete type to discover scene property references, resolving them against the <see cref="ISceneObjectRegistry"/>,
        /// and optionally walking <c>RequireComponent</c> dependency chains.
        /// </summary>
        /// <param name="data">The condition data whose concrete type is scanned for scene property reference members.
        /// If null, an empty list is returned.</param>
        /// <param name="checkRequiredComponentsToo">If <c>true</c> (default), recursively resolves
        /// <c>RequireComponent</c> / <c>RequireComponentAttribute</c> dependency chains so that locking a property
        /// also locks its required sibling properties. Set to <c>false</c> to return only the directly referenced property.</param>
        /// <returns>A list of <see cref="LockablePropertyData"/> representing every lockable property this condition
        /// references via its <see cref="SingleScenePropertyReference{T}"/> and <see cref="MultipleScenePropertyReference{T}"/>
        /// members. Never null.</returns>
        public static List<LockablePropertyData> ExtractLockablePropertiesFromCondition(IConditionData data, bool checkRequiredComponentsToo = true)
        {
            if (data == null)
            {
                return new List<LockablePropertyData>();
            }

            List<LockablePropertyData> result = new List<LockablePropertyData>();
            List<MemberInfo> memberInfo = GetAllPropertiesInGroupsFromCondition(data);

            foreach (var info in memberInfo)
            {
                ProcessSceneReferenceBase reference = ReflectionUtils.GetValueFromPropertyOrField(data, info) as ProcessSceneReferenceBase;

                if (reference == null || reference.IsEmpty())
                {
                    continue;
                }

                List<ISceneObject> sceneObjects = reference.Guids
                    .SelectMany(guid => ServiceRegistry.Get<ISceneObjectRegistry>().GetObjects(guid))
                    .Distinct()
                    .ToList();

                if (!sceneObjects.Any())
                {
                    continue;
                }

                IEnumerable<Type> refs = ExtractFittingPropertyType<ILockableProperty>(reference.GetReferenceType());

                foreach (ISceneObject sceneObject in sceneObjects)
                {
                    Type refType = refs.FirstOrDefault(sceneObject.CheckHasProperty);
                    if (refType != null)
                    {
                        IEnumerable<Type> types = new[] { refType };
                        if (checkRequiredComponentsToo)
                        {
                            types = GetDependenciesFrom<ILockableProperty>(refType);
                        }

                        foreach (Type type in types)
                        {
                            if (sceneObject.Properties.FirstOrDefault(property => property.GetType() == type) is ILockableProperty property)
                            {
                                result.Add(new LockablePropertyData(property));
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all concrete, non-abstract runtime property types that implement the given abstract
        /// reference type and are also assignable to <typeparamref name="T"/>. Filters out types from
        /// test and editor assemblies when not running in a unit test context.
        /// </summary>
        /// <param name="referenceType">An abstract type or interface (e.g., <c>IGrabbableProperty</c>) for which
        /// concrete implementations are sought. Typically the type parameter used in a
        /// <see cref="SingleScenePropertyReference{T}"/>.</param>
        /// <typeparam name="T">The lockable constraint type, typically <see cref="ILockableProperty"/>.
        /// Only implementations that are assignable to <typeparamref name="T"/> are returned.</typeparam>
        /// <returns>Concrete runtime property types that implement <paramref name="referenceType"/> and
        /// satisfy <typeparamref name="T"/>.</returns>
        public static IEnumerable<Type> ExtractFittingPropertyType<T>(Type referenceType) where T : ISceneObjectProperty
        {
            IEnumerable<Type> refs = ReflectionUtils.GetConcreteImplementationsOf(referenceType);
            refs = refs.Where(typeof(T).IsAssignableFrom);

            if (!UnitTestChecker.IsUnitTesting)
            {
                refs = refs.Where(type => type.Assembly.GetReferencedAssemblies().All(name => name.Name != "nunit.framework"));
                refs = refs.Where(type => !ShouldExcludeType(type));
            }

            return refs;
        }

        /// <summary>
        /// Reflects on the concrete type of a condition data object to discover all
        /// <see cref="SingleScenePropertyReference{T}"/> and <see cref="MultipleScenePropertyReference{T}"/>
        /// members - both properties (public and non-public) and public fields - that the condition declares.
        /// </summary>
        /// <param name="conditionData">The condition data instance whose concrete type is scanned.
        /// Must not be null.</param>
        /// <returns>A list of <see cref="MemberInfo"/> for every property or field on the condition's concrete type
        /// whose declared type is a constructed generic of <see cref="SingleScenePropertyReference{T}"/> or
        /// <see cref="MultipleScenePropertyReference{T}"/>. Never null.</returns>
        /// <remarks>
        /// This method performs a cross-product scan over two axes:
        /// <list type="bullet">
        ///   <item><description><b>Member kinds:</b> properties (with <c>BindingFlags.Public | NonPublic | Instance</c>)
        ///   and fields (with <c>BindingFlags.Public | Instance</c>). Non-public fields are intentionally excluded
        ///   because the convention expects scene property references to be public fields or any-visibility auto-properties.</description></item>
        ///   <item><description><b>Reference types:</b> <see cref="SingleScenePropertyReference{T}"/> for single-target
        ///   references and <see cref="MultipleScenePropertyReference{T}"/> for multi-target references.</description></item>
        /// </list>
        /// <para>
        /// The scan uses <c>GetMembers</c> to retrieve all members matching each flag combination, then filters by
        /// checking <c>IsConstructedGenericType</c> and comparing the generic type definition against the two known
        /// reference types. <c>GetMembers</c> is used instead of separate <c>GetProperties</c> + <c>GetFields</c> calls
        /// to avoid redundant reflection lookups and to keep the query unified.
        /// </para>
        /// <para>
        /// Because it only checks <c>IsConstructedGenericType</c> - meaning the type parameter must be supplied -
        /// open generics like <c>SingleScenePropertyReference&lt;&gt;</c> (without a type argument) are naturally excluded.
        /// </para>
        /// <para>
        /// This method has no explicit null guard on <paramref name="conditionData"/>; passing null will result in a
        /// <see cref="NullReferenceException"/> from <c>GetType()</c>. Callers are expected to have validated non-null
        /// before calling.
        /// </para>
        /// </remarks>
        private static List<MemberInfo> GetAllPropertiesInGroupsFromCondition(IConditionData conditionData)
        {
            Type conditionType = conditionData.GetType();
            Type[] referenceTypes = { typeof(SingleScenePropertyReference<>), typeof(MultipleScenePropertyReference<>) };
            (BindingFlags flags, Func<MemberInfo, Type> getMemberType)[] memberKinds =
            {
                (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, GetMemberType),
                (BindingFlags.Instance | BindingFlags.Public, GetMemberType),
            };

            return referenceTypes
                .SelectMany(refType => memberKinds
                    .SelectMany(kind => conditionType
                        .GetMembers(kind.flags)
                        .Where(m => m is PropertyInfo or FieldInfo
                                    && kind.getMemberType(m).IsConstructedGenericType
                                    && kind.getMemberType(m).GetGenericTypeDefinition() == refType)
                        .Cast<MemberInfo>()))
                .ToList();
        }

        private static Type GetMemberType(MemberInfo m) => m is PropertyInfo p ? p.PropertyType : ((FieldInfo)m).FieldType;

        /// <summary>
        /// Recursively resolves the transitive closure of <c>RequireComponent</c> dependencies for a given property type.
        /// If property type A is marked as requiring component type B (and B requires C), the result includes {A, B, C}.
        /// </summary>
        /// <param name="processProperty">The property type whose dependency chain is resolved.
        /// Included in the result set as the root of the chain.</param>
        /// <typeparam name="T">The base type constraint. Only required types that are subclasses of
        /// <typeparamref name="T"/> are followed; types outside the hierarchy are ignored.</typeparam>
        /// <returns>A lazy enumeration of all types in the transitive dependency chain, including
        /// <paramref name="processProperty"/> itself. Duplicates are not de-duplicated - the caller
        /// is responsible for deduplication if needed.</returns>
        private static IEnumerable<Type> GetDependenciesFrom<T>(Type processProperty) where T : ISceneObjectProperty
        {
            foreach (var requiredComponent in ResolveRequiredComponents?.Invoke(processProperty) ?? Enumerable.Empty<Type>())
                if (requiredComponent != null && requiredComponent.IsSubclassOf(typeof(T)))
                    foreach (var dependency in GetDependenciesFrom<T>(requiredComponent))
                        yield return dependency;

            yield return processProperty;
        }
    }
}