using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API.Controllers;

[ApiController]
[Route("image")]
public class UploadImageController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly IImageSender _imageSender;

    public UploadImageController(ILogger<UploadImageController> logger, IImageSender imageSender)
    {
        _logger = logger;
        _imageSender = imageSender;
    }

    [HttpPost(Name = "UploadImage")]
    public async Task<IActionResult> Post(IFormFile file, string ipAddress, int port = 21324, int width = 32, int height = 8, int wait = 1, int pauseBetweenFrames = 100, int iterations = 1)
    {
        _logger.LogInformation("UpdateImage called");

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var filePath = System.IO.Path.Combine(Path.GetTempPath(), file.FileName);

        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        _imageSender.Send(
            ipAddress,
            port,
            filePath,
            new Size(width, height),
            0,
            (byte)wait,
            pauseBetweenFrames,
            iterations
        );
        // TODO: Process the file here
        return Ok("File uploaded successfully");
    }
}