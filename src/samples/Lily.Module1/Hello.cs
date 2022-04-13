using Lily.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lily.Module1
{
    [Scoped]
    public class Hello : IHello
    {
        private readonly Option1 _option1;

        public Hello(IOptionsMonitor<Option1> options)
        {
            _option1 = options.CurrentValue;
        }

        public string SayHello()
        {
            return _option1.Value;
        }
    }

    [DependencyInjection(Ignored = true)]
    public class IgnoredHello : Hello
    {
        public IgnoredHello(IOptionsMonitor<Option1> options) : base(options)
        {
        }
    }

    [Transient]
    public class TransitHello : Hello
    {
        public TransitHello(IOptionsMonitor<Option1> options) : base(options)
        {
        }
    }
}
