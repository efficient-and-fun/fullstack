using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Common;

namespace WebApi;

[ApiController]
public abstract class BaseController : Controller
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
}