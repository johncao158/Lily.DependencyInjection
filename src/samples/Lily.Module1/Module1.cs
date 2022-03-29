using Lily.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lily.Module1
{
    public class Module1 : IModule
    {
        public void Configure(IServiceCollection services, IConfiguration? configuration)
        {
            if (configuration == null)
            {
                services.Configure<Option1>(option => option.Value = "");
            }
            else
            {
                services.Configure<Option1>(configuration.GetSection("Module1:Option1"));
            }
        }
    }
}
