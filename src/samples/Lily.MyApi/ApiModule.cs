using Lily.DependencyInjection;

namespace Lily.MyApi
{
    [DependsOn(
        typeof(Module1.Module1),
        typeof(Module2.Module2))]
    public class ApiModule : IModule
    {
        public void Configure(IServiceCollection services, IConfiguration? configuration)
        {
        }
    }
}
