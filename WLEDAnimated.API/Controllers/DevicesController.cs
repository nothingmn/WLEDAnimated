using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace WLEDAnimated.API.Controllers;

[ApiController]
public class DevicesController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly WledDeviceDiscovery _discovery;

    public DevicesController(ILogger<UploadImageController> logger, WledDeviceDiscovery discovery)
    {
        _logger = logger;
        _discovery = discovery;
    }

    [HttpGet()]
    [Route("devices")]
    public Task<List<WLEDDevice>> Devices()
    {
        _logger.LogInformation("devices called");

        return Task.FromResult(_discovery.Devices);
    }
}