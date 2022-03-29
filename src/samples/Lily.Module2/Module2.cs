using Lily.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lily.Module2
{
    public class Module2 : IModule
    {
        public void Configure(IServiceCollection services, IConfiguration? configuration)
        {
            if (configuration == null)
            {
                services.Configure<Option2>(option => option.Value = "");
            }
            else
            {
                services.Configure<Option2>(configuration.GetSection("Module2:Option2"));
            }
        }
    }
}
