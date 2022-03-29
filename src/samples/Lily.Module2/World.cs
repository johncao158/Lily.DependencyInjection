using Lily.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lily.Module2
{
    [Transient(typeof(IWorld))]
    public class World : IWorld
    {
        private readonly Option2 _option2;

        public World(IOptionsMonitor<Option2> options)
        {
            _option2 = options.CurrentValue;
        }

        public string SayWorld()
        {
            return _option2.Value;
        }
    }
}
