using System.Net;
using System.Runtime.InteropServices.Marshalling;
using AnimationCore;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Animation;

namespace WLEDAnimated.API.Controllers;

[ApiController]
[Route("animation")]
public class UploadAnimationController : ControllerBase
{
    private readonly ILogger<UploadAnimationController> _logger;

    public UploadAnimationController(ILogger<UploadAnimationController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "Upload Animation")]
    public async Task<IActionResult> Post(IFormFile file)
    {
        _logger.LogInformation("UploadAnimationController called");

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var filePath = System.IO.Path.Combine(Path.GetTempPath(), file.FileName);

        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var loader = new WLEDAnimationLoader();
        var animation = await loader.LoadAnimation(filePath);

        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<AnimationProgress>(p =>
            {
                Console.WriteLine($"Overall Progress: {p.OverallProgress}%");
                Console.WriteLine($"Current Transition: {p.CurrentTransition}, Progress: {p.TransitionProgress}%");
            });

            // Start the animation asynchronously
            await animation.StartAsync(cancellationTokenSource.Token, progress);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Animation was canceled.");
        }
        return Ok("File uploaded successfully");
    }
}