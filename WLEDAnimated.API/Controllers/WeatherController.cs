using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly AssetManager _assetManager;
    private readonly IImageSender _sender;

    public WeatherController(ILogger<UploadImageController> logger, AssetManager assetManager, IImageSender sender)
    {
        _logger = logger;
        _assetManager = assetManager;
        _sender = sender;
    }

    [HttpGet("change", Name = "change")]
    public async Task<IActionResult> Change(string ipAddress, int port = 21324, int width = 32, int height = 8, string conditions = "sunny")
    {
        _logger.LogInformation("UpdateImage called");

        var file = _assetManager.GetFileByAssetTypeAndResolutionAndName(AssetTypes.Weather, width, height, conditions);

        if (file == null)
        {
            return BadRequest($"No weather animation on file for: {conditions} for the dimensions of {height}x{width}.");
        }
        var filePath = System.IO.Path.Combine(Path.GetTempPath(), file.Name);
        file.CopyTo(filePath);

        _sender.Send(
            ipAddress,
            port,
            filePath,
            new Size(width, height),
            0,
            (byte)1,
            500,
            1
        );
        return Ok(conditions);
    }

    [HttpGet("sunny", Name = "sunny")]
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

        _sender.Send(
            ipAddress,
            port,
            filePath,
            new Size(width, height),
            0,
            (byte)1,
            500,
            1
        );
        return Ok("Its sunny");
    }
}