using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace WLEDAnimated.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly AssetManager _assetManager;

    public WeatherController(ILogger<UploadImageController> logger, AssetManager assetManager)
    {
        _logger = logger;
        _assetManager = assetManager;
    }

    [HttpGet("[controller]/weather/sunny", Name = "sunny")]
    public async Task<IActionResult> Sunny(string ipAddress, int port = 21324, int width = 32, int height = 8)
    {
        _logger.LogInformation("UpdateImage called");

        var file = _assetManager.GetFileByAssetTypeAndResolutionAndName(AssetTypes.Weather, width, height, "sunny");

        if (file == null)
        {
            return BadRequest("Nothing to do here.");
        }
        var filePath = System.IO.Path.Combine(Path.GetTempPath(), file.Name);
        file.CopyTo(filePath);

        var sender = new ImageUDPSender();

        sender.Send(
            ipAddress,
            port,
            filePath,
            new Size(width, height),
            0,
            (byte)1,
            500,
            1
        );
        // TODO: Process the file here
        return Ok("Its sunny");
    }
}