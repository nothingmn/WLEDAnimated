using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly AssetManager _assetManager;
    private readonly IImageSender _sender;

    public TestController(ILogger<UploadImageController> logger, AssetManager assetManager, IImageSender sender)
    {
        _logger = logger;
        _assetManager = assetManager;
        _sender = sender;
    }

    [HttpGet("test", Name = "test")]
    public async Task<IActionResult> Change(string ipAddress, int port = 21324, int width = 32, int height = 8, int pauseBetweenFrames = 100, int iterations = 1)
    {
        _logger.LogInformation("Test Change called");

        var file = _assetManager.GetFileByAssetTypeAndResolutionAndName(AssetTypes.Test, width, height, "8x32.test");

        if (file == null)
        {
            return BadRequest($"No test animation on file for: 8x32.test for the dimensions of {height}x{width}.");
        }
        var filePath = System.IO.Path.Combine(Path.GetTempPath(), file.Name);
        if (!System.IO.File.Exists(filePath))
            file.CopyTo(filePath, false);

        _sender.Send(
            ipAddress,
            port,
            filePath,
            new Size(width, height),
            0,
            (byte)1,
            pauseBetweenFrames,
            iterations
        );
        return Ok("Done");
    }
}