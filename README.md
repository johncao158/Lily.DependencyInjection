# Lily.DependencyInjection
============================= 

Extends .net dependency injection.

## samples

- ### IHello.cs
```cs
namespace Lily.Module1
{
    public interface IHello
    {
        string SayHello();
    }
}
```

- ### Hello.cs
```cs
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
}
```

- ### Option1.cs
```cs
namespace Lily.Module1
{
    public class Option1
    {
        public string Value { get; set; } = string.Empty;
    }
}
```

- ### Module1.cs
```cs
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
```

- ### ApiModule.cs
```cs
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
```

- ### Program.cs
```cs
using Lily.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// load current assembly
builder.Services.Load(typeof(Program).Assembly, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

```

- ### TestController.cs
```cs
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
```

- ### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Module1": {
    "Option1": {
      "Value": "hello"
    }
  },
  "Module2": {
    "Option2": {
      "Value": "world"
    }
  }
}
```

- ### Test API
```
curl -X 'GET' \
  'https://localhost:7012/api/Test' \
  -H 'accept: text/plain'

[Response body]:
hello world
```