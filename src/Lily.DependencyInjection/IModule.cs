using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lily.DependencyInjection
{
    /// <summary>
    /// Represents a module.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Configures <paramref name="services"/> with specified <paramref name="configuration"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        void Configure(IServiceCollection services, IConfiguration? configuration);
    }
}
