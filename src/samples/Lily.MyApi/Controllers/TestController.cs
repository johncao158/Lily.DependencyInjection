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

        public TestController(IHello hello, IWorld world)
        {
            _hello = hello;
            _world = world;
        }

        [HttpGet]
        public async Task<string> Index()
        {
            return await Task.FromResult($"{_hello.SayHello()} {_world.SayWorld()}");
        }
    }
}
