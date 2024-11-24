using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthcheckController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly DapperContext _dapper;
    [HttpGet]
    public IActionResult Get()
    {
        // TODO: yank out, we don't want to expose information about our running environment. This was just a convenient place to set up a little data for index.html reloading
        bool isDev;
        bool database;
        try
        {
            _dapper.CreateConnection();
            database = true;
        }
        catch (Exception exc)
        {
            database = false;
            _logger.LogDebug("@{exc}", exc);
        }
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is string aspnetcoreEnv)
        {
            _logger.LogDebug("ASPNETCORE_ENVIRONMENT={aspnetcoreEnv}", aspnetcoreEnv);
            if (aspnetcoreEnv == "Development")
            {
                isDev = true;
            }
            else
            {
                isDev = false;
            }
        }
        else
        {
            isDev = false;
        }
        return Ok(new { IsDev = isDev, Database = database });
    }

    public HealthcheckController(ILogger<HealthcheckController> logger, DapperContext dapper)
    {
        _logger = logger;
        _dapper = dapper;
    }
}