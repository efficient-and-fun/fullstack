using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Common;

namespace WebApi.Api.Controller;

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