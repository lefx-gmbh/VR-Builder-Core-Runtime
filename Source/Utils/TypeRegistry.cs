// Copyright (c) 2021-2026 MindPort GmbH

using System;
using System.Collections.Generic;
using System.Linq;

namespace VRBuilder.Core.Utils
{
    /// <summary>
    /// AOT-safe replacement for reflection-based "find all concrete types assignable to X" lookups.
    ///
    /// Populated entirely by generated code (see VRBuilder.Catalog.Generator's TypeRegistryGenerator),
    /// via a <see cref="System.Runtime.CompilerServices.ModuleInitializerAttribute"/> emitted into each
    /// compiling assembly. Every public/internal concrete class in that assembly registers itself
    /// against every base type and interface it implements, so lookups never need
    /// <see cref="System.Reflection.Assembly.GetTypes"/> or whole-AppDomain scanning - both of which
    /// are unreliable once an app is trimmed or NativeAOT-published.
    /// </summary>
    public static class TypeRegistry
    {
        private static readonly Dictionary<Type, List<Type>> ImplementationsByBaseType = new();

        /// <summary>
        /// Registers <paramref name="concreteType"/> as an implementation of every type in
        /// <paramref name="assignableTypes"/> (its base classes and interfaces). Called only by
        /// generated module initializers - not intended to be called by hand.
        /// </summary>
        public static void Register(Type concreteType, params Type[] assignableTypes)
        {
            foreach (Type baseType in assignableTypes)
            {
                if (!ImplementationsByBaseType.TryGetValue(baseType, out List<Type> implementations))
                {
                    implementations = new List<Type>();
                    ImplementationsByBaseType[baseType] = implementations;
                }

                implementations.Add(concreteType);
            }
        }

        /// <summary>
        /// Returns all concrete types registered as implementations of <paramref name="baseType"/>.
        /// </summary>
        public static IEnumerable<Type> GetConcreteImplementationsOf(Type baseType)
        {
            return ImplementationsByBaseType.TryGetValue(baseType, out List<Type> implementations)
                ? implementations
                : Enumerable.Empty<Type>();
        }
    }
}
