using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;

namespace WebApi.Api.Controller;

/// <summary>
/// Provides an abstract base class for API controllers, supplying shared dependencies 
/// such as a logger, configuration, and database context.
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> Logger;
    protected readonly IConfiguration Configuration;

    protected readonly EfDbContext Context;

    protected BaseController(ILogger<BaseController> logger, IConfiguration configuration, EfDbContext context)
    {
        Logger = logger;
        Configuration = configuration;
        Context = context;
    }
}