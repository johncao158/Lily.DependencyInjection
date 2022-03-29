using Microsoft.Extensions.DependencyInjection;

namespace Lily.DependencyInjection
{
    /// <summary>
    /// Defines a dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyInjectionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionAttribute"/> class.
        /// </summary>
        public DependencyInjectionAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionAttribute"/> class 
        /// with specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">The specified <see cref="ServiceLifetime"/>.</param>
        public DependencyInjectionAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionAttribute"/> class
        /// with specified <paramref name="serviceType"/> and <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="serviceType">The type of the service to be injected for.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/>.</param>
        public DependencyInjectionAttribute(
            Type serviceType,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        /// <summary>
        /// Gets the service type.
        /// </summary>
        public Type? ServiceType { get; }

        /// <summary>
        /// Gets the <see cref="ServiceLifetime"/>.
        /// </summary>
        public ServiceLifetime Lifetime { get; }
    }

    /// <summary>
    /// Defines a <see cref="ServiceLifetime.Transient"/> dependency injection.
    /// </summary>
    public class TransientAttribute : DependencyInjectionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientAttribute"/> class.
        /// </summary>
        public TransientAttribute() : base(ServiceLifetime.Transient)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientAttribute"/> class
        /// with specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the service to be injected for.</param>
        public TransientAttribute(Type serviceType)
            : base(serviceType, ServiceLifetime.Transient)
        {
        }
    }

    /// <summary>
    /// Defines a <see cref="ServiceLifetime.Scoped"/> dependency injection.
    /// </summary>
    public class ScopedAttribute : DependencyInjectionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedAttribute"/> class.
        /// </summary>
        public ScopedAttribute() : base(ServiceLifetime.Scoped)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedAttribute"/> class
        /// with specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the service to be injected for.</param>
        public ScopedAttribute(Type serviceType)
            : base(serviceType, ServiceLifetime.Scoped)
        {
        }
    }

    /// <summary>
    /// Defines a <see cref="ServiceLifetime.Singleton"/> dependency injection.
    /// </summary>
    public class SingletonAttribute : DependencyInjectionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonAttribute"/> class.
        /// </summary>
        public SingletonAttribute() : base(ServiceLifetime.Singleton)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonAttribute"/> class
        /// with specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">The type of the service to be injected for.</param>
        public SingletonAttribute(Type serviceType)
            : base(serviceType, ServiceLifetime.Singleton)
        {
        }
    }
}