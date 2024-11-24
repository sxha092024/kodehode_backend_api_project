using System.Data.SqlTypes;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Structs;

namespace Api;

[ApiController]
[Route("api/v1")]
public class Apiv1Controller : ControllerBase
{

    private readonly DapperContext _dapper;
    private readonly ILogger _logger;

    [HttpGet("get")]
    public IActionResult Get([FromQuery(Name = "guid")]string requestedGuid)
    {
        _logger.LogDebug("{Request}", Request);
        var connection = _dapper.CreateConnection();
        var command = "SELECT * FROM UploadTable WHERE Guid like @Guid";
        try
        {
            var result = connection.QuerySingle<UploadAsset>(command, new { Guid = requestedGuid });
            _logger.LogDebug("Result from query: {@result}", result);
            // var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            // response.Content = new StreamContent(new MemoryStream(result.Data));
            // response.Headers.Add("filename", result.Name);
            // response.Headers.Add("guid", result.Guid.ToString());
            // response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(result.ContentType);
            // return response;
            return File(new MemoryStream(result.Data), result.ContentType, result.Name);
        }
        catch (Exception exc)
        {
            _logger.LogError("{@exc}", exc);
            throw;
        }
    }

    [HttpPost("upload")]
    public IActionResult Upload(IFormFile file)
    {
        var fileInfo = new { Name = file.FileName, Length = file.Length, ContentType = file.ContentType, Headers = file.Headers };
        _logger.LogDebug("{@fileInfo}", fileInfo);

        byte[] bytes;
        using (MemoryStream ms = new())
        {
            _logger.LogInformation("Opened memory stream");
            using (var stream = file.OpenReadStream())
            {
                _logger.LogInformation("Opened read stream");
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }
        }
        _logger.LogDebug("bytes = [{@bytes}]", bytes);

        var connection = _dapper.CreateConnection();

        var command = "INSERT INTO UploadTable (Data, ContentType, Name, Guid) VALUES (@Data, @ContentType, @Name, @Guid)";
        var DbOject = new
        {
            Data = bytes,
            file.ContentType,
            Name = file.FileName,
            Guid = Guid.NewGuid()
        };
        connection.Execute(command, DbOject);

        return Ok(new { guid = DbOject.Guid.ToString() });

    }

    public Apiv1Controller(DapperContext dapper, ILogger<Apiv1Controller> ilogger)
    {
        _dapper = dapper;
        _logger = ilogger;
    }
}