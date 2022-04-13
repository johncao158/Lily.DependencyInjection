using Lily.Module1;
using Lily.Module2;
using Microsoft.AspNetCore.Mvc;

namespace Lily.MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IHello _hello;
        private readonly IWorld _world;
        private readonly IgnoredHello _ignoredHello;

        public TestController(IHello hello, IWorld world, IgnoredHello ignoredHello)
        {
            _hello = hello;
            _world = world;
            _ignoredHello = ignoredHello;
        }

        [HttpGet]
        public async Task<string> Index()
        {
            return await Task.FromResult($"{_hello.SayHello()} {_world.SayWorld()}");
        }
    }
}
