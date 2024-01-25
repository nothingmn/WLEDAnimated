using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlImageController : ControllerBase
{
    private readonly ILogger<UploadImageController> _logger;
    private readonly IImageSender _sender;

    public UrlImageController(ILogger<UploadImageController> logger, IImageSender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpPost()]
    public async Task<IActionResult> Post(string url, string ipAddress, int port = 21324, int width = 32, int height = 8, int wait = 1, int pauseBetweenFrames = 100, int iterations = 1)
    {
        _logger.LogInformation("UpdateImage called");

        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
        System.Uri uri;
        if (System.Uri.TryCreate(url, UriKind.Absolute, out uri))
        {
            //download image from url
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var filename = Path.GetFileName(uri.LocalPath);
                if (string.IsNullOrWhiteSpace(filename))
                {
                    filename = Guid.NewGuid().ToString();
                    response.Content.Headers.TryGetValues("Content-Type", out var contentTypes);
                    if (contentTypes != null && contentTypes.Any())
                    {
                        var contentType = contentTypes.First();
                        var extension = contentType.Split('/').Last();
                        filename = $"{filename}.{extension}";
                    }
                }

                var filePath = System.IO.Path.Combine(Path.GetTempPath(), filename);
                var fileInfo = new System.IO.FileInfo(filePath);
                if (string.IsNullOrWhiteSpace(fileInfo.Extension)) filePath = $"{filePath}.gif";

                using (var fileStream = System.IO.File.Create(filePath))
                {
                    await stream.CopyToAsync(fileStream);
                }

                _sender.Send(ipAddress, port, filePath, new Size(width, height), 0, (byte)wait, pauseBetweenFrames, iterations);
            }
        }

        return Ok("File downloaded successfully");
    }
}