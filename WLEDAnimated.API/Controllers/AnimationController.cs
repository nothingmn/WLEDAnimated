using System.Net;
using System.Runtime.InteropServices.Marshalling;
using AnimationCore;
using AnimationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using WLEDAnimated.Animation;

namespace WLEDAnimated.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AnimationController : ControllerBase
{
    private readonly ILogger<AnimationController> _logger;
    private readonly AnimationManager _animationManager;

    public AnimationController(ILogger<AnimationController> logger, AnimationManager animationManager)
    {
        _logger = logger;
        _animationManager = animationManager;
    }

    [HttpPost()]
    [Route("play")]
    public async Task<IAnimation> Play(string name)
    {
        return await _animationManager.PlayAnimation(name);
    }

    [HttpPost()]
    [Route("upload")]
    public async Task<IAnimation> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File is empty");

        var animation = await _animationManager.UploadAnimation(file.OpenReadStream(), file.FileName);

        return animation;
    }

    [HttpPost()]
    [Route("animate")]
    public async Task<IAnimation> UploadAndPlay(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File is empty");

        return await _animationManager.UploadAndPlayAnimation(file.OpenReadStream(), file.FileName);
    }
}