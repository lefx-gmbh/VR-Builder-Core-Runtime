using System;
using System.Collections.Generic;
using System.Linq;
using VRBuilder.Core.Registry;

namespace VRBuilder.Core.Runtime.Registry
{
    /// <summary>
    /// Static service locator for the engine-agnostic core. Services implementing
    /// <see cref="IService{TConfig}"/> are registered — optionally together with their configuration —
    /// and later resolved by type. Default implementations are wired up at startup by the engine-specific
    /// registry loader (e.g. <c>ServiceRegistryLoader</c> in TinkerFlow).
    /// </summary>
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> services = new();

        /// <summary>
        /// Registers a service instance under the given type.
        /// </summary>
        /// <typeparam name="T">The type under which the service is registered and later resolved.</typeparam>
        /// <param name="service">The service instance to register. If <c>null</c>, the registration is ignored and the error is logged.</param>
        public static void Register<T>(T? service) where T : class
        {
            if (service != null)
                services[typeof(T)] = service;
            else
                ForwardingLogger.LogException(new ArgumentNullException(nameof(service)));
        }

        /// <summary>
        /// Registers a service together with its configuration. Calls <c>SetConfiguration</c> on the service
        /// before storing it, so the service is fully configured from the start.
        /// </summary>
        /// <typeparam name="TService">The type under which the service is registered and later resolved.</typeparam>
        /// <typeparam name="TConfig">The configuration type the service accepts.</typeparam>
        /// <param name="service">The service instance to register. If <c>null</c>, the registration is ignored and the error is logged.</param>
        /// <param name="config">The configuration to apply to <paramref name="service"/> before storing it.</param>
        public static void Register<TService, TConfig>(TService? service, TConfig? config)
            where TService : class, IService<TConfig>
            where TConfig : IServiceConfiguration
        {
            if (service == null)
            {
                ForwardingLogger.LogException(new ArgumentNullException(nameof(service)));
                return;
            }

            service.SetConfiguration(config);
            services[typeof(TService)] = service;
        }

        /// <summary>
        /// Returns the registered service that can be returned as the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to resolve. The registered service type may be a base type of <paramref name="T"/>.</typeparam>
        /// <returns>The registered service instance cast to <typeparamref name="T"/>, or <c>null</c> if no matching service is registered.</returns>
        public static T Get<T>() where T : class
        {
            if (services.TryGetValue(typeof(T), out var service))
                return service as T;

            foreach (var (key, value) in services) //e.g. DefaultProcessRunner won't be found above, so we have to allow subtypes as well.
            {
                if (key.IsAssignableFrom(typeof(T)))
                    return value as T;
            }

            return null;
        }

        /// <summary>
        /// Returns whether a service assignable from the given type is registered.
        /// </summary>
        /// <typeparam name="T">The type of the service to look for.</typeparam>
        /// <returns><c>true</c> if a service whose registered type can be assigned from <typeparamref name="T"/> is registered; otherwise, <c>false</c>.</returns>
        public static bool Has<T>() => services.Keys.Any(key => key.IsAssignableFrom(typeof(T)));
    }
}