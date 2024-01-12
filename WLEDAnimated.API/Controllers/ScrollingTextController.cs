using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API.Controllers;

[ApiController]
public class ScrollingTextController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly IWLEDApiManager _apiManager;

    public ScrollingTextController(ILogger<UploadImageController> logger, IWLEDApiManager apiManager)
    {
        _logger = logger;
        _apiManager = apiManager;
    }

    [HttpGet("scroll")]
    public async Task<IActionResult> Scroll(string ipAddress, string text, int speed)
    {
        _logger.LogInformation("Scrolling text called");
        await _apiManager.Connect(ipAddress);
        await _apiManager.ScrollingText(text, speed);

        return Ok("Done");
    }
}