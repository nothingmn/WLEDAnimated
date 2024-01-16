using System.Net;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace WLEDAnimated.API.Controllers;

[ApiController]
[Route("image")]
public class UploadImageController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;

    public UploadImageController(ILogger<UploadImageController> logger)
    {
        _logger = logger;
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

        var sender = new ImageUDPSender();

        sender.Send(
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