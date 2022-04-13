using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace Lily.DependencyInjection
{
    /// <summary>
    /// Delegate to filter the implemented interface type for a class.
    /// </summary>
    /// <param name="interfaceType">The implemented interface type.</param>
    /// <param name="classType">The class type.</param>
    /// <returns>true if the interface type passes the filter; otherwise, false.</returns>
    public delegate bool InterfaceFilter(Type interfaceType, Type classType);

    /// <summary>
    /// Extends the <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        // set to store the loaded assemblies.
        [ThreadStatic]
        private static HashSet<int> s_loadedAssemblies = new();

        // set to store the loaded modules.
        [ThreadStatic]
        private static HashSet<int> s_loadedModules = new();

        /// <summary>
        /// Default <see cref="InterfaceFilter"/> when register injections for classes and their implemented interfaces.
        /// </summary>
        public static readonly InterfaceFilter DefaultInterfaceFilter =
            (t1, t2) => t2.Name.Contains(t1.Name.TrimStart('I'));

        /// <summary>
        /// Loads a collection types with specified <paramref name="configuration"/>, and <paramref name="filter"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="types">The types to be loaded.</param>
        /// <param name="configuration">Optional <see cref="IConfiguration"/>.</param>
        /// <param name="filter">Optional <see cref="InterfaceFilter"/>. (Default is <see cref="DefaultInterfaceFilter"/>)</param>
        /// <returns>The <paramref name="services"/>.</returns>
        /// <remarks>
        /// <see cref="Load(IServiceCollection, IEnumerable{Type}, IConfiguration?, InterfaceFilter?)"/> will try to
        /// register injections using the <see cref="DependencyInjectionAttribute"/>s defined on the <paramref name="types"/>.
        /// 
        /// Also, it will also try to load <see cref="IModule"/>s if found among the <paramref name="types"/>.
        /// </remarks>
        public static IServiceCollection Load(
            this IServiceCollection services,
            IEnumerable<Type> types,
            IConfiguration? configuration = null,
            InterfaceFilter? filter = null)
        {
            var interfaceFilter = filter ?? DefaultInterfaceFilter;
            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract || type.IsNotPublic)
                {
                    continue;
                }

                var isDependencyInjectAttributeLoaded = false;
                var isDependsOnAttributeLoaded = false;

                // check attributes that are not inherited
                LoadAttributes(Attribute.GetCustomAttributes(type, false));

                // check inherited attributes
                if (!isDependencyInjectAttributeLoaded || !isDependsOnAttributeLoaded)
                {
                    LoadAttributes(Attribute.GetCustomAttributes(type, true));
                }

                // load module
                if ((typeof(IModule).IsAssignableFrom(type)) &&
                    s_loadedModules.Add(type.GetHashCode()))
                {
                    try
                    {
                        if (Activator.CreateInstance(type) is IModule module)
                        {
                            Debug.WriteLine($"Loading [{type.FullName}]");

                            module.Configure(services, configuration);

                            Debug.WriteLine($"[{type.FullName}] Loaded");
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                void LoadAttributes(IEnumerable<Attribute> attributes)
                {
                    foreach (var attr in attributes)
                    {
                        if (attr is DependencyInjectionAttribute diAttribute)
                        {
                            if (isDependencyInjectAttributeLoaded)
                            {
                                continue;
                            }

                            if (diAttribute.Ignored)
                            {
                                isDependencyInjectAttributeLoaded =true;
                                continue;
                            }

                            var lifetime = diAttribute.Lifetime;
                            if (diAttribute.ServiceType == null)
                            {
                                // add for interfaces
                                type.FindInterfaces((serviceType, obj) =>
                                {
                                    var implementationType = (Type)obj!;
                                    if (interfaceFilter(serviceType, implementationType))
                                    {
                                        if (serviceType.IsGenericType)
                                        {
                                            serviceType = serviceType.GetGenericTypeDefinition();
                                        }

                                        services.TryAdd(serviceType, implementationType, lifetime);

                                        return true;
                                    }
                                    return false;
                                }, type);

                                // add type
                                services.TryAdd(type, type, lifetime);
                            }
                            else
                            {
                                services.TryAdd(diAttribute.ServiceType, type, lifetime);
                            }

                            isDependencyInjectAttributeLoaded = true;
                        }
                        else if (attr is DependsOnAttribute dependsAttribute)
                        {
                            if (isDependsOnAttributeLoaded)
                            {
                                continue;
                            }

                            foreach (var dependedType in dependsAttribute.DependedTypes)
                            {
                                if (dependedType != null)
                                {
                                    Load(services, dependedType.Assembly, configuration, filter);
                                }
                            }

                            isDependsOnAttributeLoaded = true;
                        }
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// Loads a <paramref name="assembly"/> with specified <paramref name="configuration"/> and <paramref name="filter"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="assembly">The specified <see cref="Assembly"/>.</param>
        /// <param name="configuration">The specified <see cref="IConfiguration"/>.</param>
        /// <param name="filter">The <see cref="InterfaceFilter"/>. (Default is <see cref="DefaultInterfaceFilter"/>)</param>
        /// <returns>The <paramref name="services"/>.</returns>
        public static IServiceCollection Load(
            this IServiceCollection services,
            Assembly assembly,
            IConfiguration? configuration = null,
            InterfaceFilter? filter = null)
        {
            if (s_loadedAssemblies.Add(assembly.GetHashCode()))
            {
                Debug.WriteLine($"Loading [{assembly.FullName}]");

                Load(services, assembly.GetTypes(), configuration, filter);

                Debug.WriteLine($"[{assembly.FullName}] Loaded");
            }
            return services;
        }

        /// <summary>
        /// Tries to add an injection for specified <paramref name="serviceType"/> with specified 
        /// <paramref name="implementationType"/> and <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="serviceType">The service type to be injected for.</param>
        /// <param name="implementationType">The implementation type to be injected.</param>
        /// <param name="lifetime">The injection <see cref="ServiceLifetime"/>.</param>
        /// <returns>The <paramref name="services"/>.</returns>
        public static IServiceCollection TryAdd(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime)
        {
            if (!services.Any(d => d.ServiceType == serviceType && d.ImplementationType == implementationType))
            {
                services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            }

            return services;
        }
    }
}
