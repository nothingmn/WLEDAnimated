using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API.Controllers;

[ApiController]
public class VersionController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly Version _version;

    public VersionController(ILogger<UploadImageController> logger, Version version)
    {
        _logger = logger;
        _version = version;
    }

    [HttpGet("version")]
    public async Task<IActionResult> Version()
    {
        _logger.LogInformation("Version API Called");
        return Ok(_version);
    }
}