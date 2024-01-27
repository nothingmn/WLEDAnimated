using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API.Controllers;

[ApiController]
public class ScrollingTextController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly IWLEDApiManager _apiManager;
    private readonly IEnumerable<IScrollingTextPlugin> _plugins;

    public ScrollingTextController(ILogger<UploadImageController> logger, IWLEDApiManager apiManager, IEnumerable<IScrollingTextPlugin> plugins)
    {
        _logger = logger;
        _apiManager = apiManager;
        _plugins = plugins;
    }

    [HttpGet("scroll")]
    public async Task<IActionResult> Scroll(string ipAddress, string text, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate)
    {
        _logger.LogInformation("Scrolling text called");
        await _apiManager.Connect(ipAddress);
        await _apiManager.ScrollingText(text, speed, yOffSet, trail, fontSize, rotate);

        return Ok("Done");
    }

    [HttpGet("scrolldata")]
    public async Task<IActionResult> ScrollData(string ipAddress, string scrollingTextPluginName, string? scrollingTextPluginPayload, int? speed, int? yOffSet, int? trail, int? fontSize, int? rotate)
    {
        _logger.LogInformation("Scrolling text called");
        await _apiManager.Connect(ipAddress);
        await _apiManager.ScrollingText(scrollingTextPluginName, scrollingTextPluginPayload, speed, yOffSet, trail, fontSize, rotate);

        return Ok("Done");
    }

    [HttpGet("plugins")]
    public Task<IEnumerable<string>> GetPlugins()
    {
        _logger.LogInformation("Get Plugins called");

        if (_plugins == null) return Task.FromResult((new List<string>()).AsEnumerable());

        return Task.FromResult((from p in _plugins select p.GetType().Name));
    }
}