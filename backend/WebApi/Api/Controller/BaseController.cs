using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Common;

namespace WebApi;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> _logger;
    protected readonly IConfiguration _configuration;

    protected readonly EfDbContext _context;

    protected BaseController(ILogger<BaseController> logger, IConfiguration configuration, EfDbContext context)
    {
        _logger = logger;
        _configuration = configuration;
        _context = context;
    }


    protected int GetUserId()
    {
        var identityClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (identityClaim == null) return -1;
        return int.Parse(identityClaim.Value);
    }
}