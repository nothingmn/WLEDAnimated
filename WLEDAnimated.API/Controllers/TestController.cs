using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace WLEDAnimated.API.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly AssetManager _assetManager;

    public TestController(ILogger<UploadImageController> logger, AssetManager assetManager)
    {
        _logger = logger;
        _assetManager = assetManager;
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
        file.CopyTo(filePath);

        var sender = new ImageUDPSender();

        sender.Send(
            ipAddress,
            port,
            filePath,
            new Size(width, height),
            0,
            (byte)1,
            pauseBetweenFrames,
            iterations
        );
        // TODO: Process the file here
        return Ok("Done");
    }
}