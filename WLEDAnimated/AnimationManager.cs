using AnimationCore.Interfaces;
using AnimationCore;
using System.Runtime.CompilerServices;
using WLEDAnimated.Animation;
using System.IO;
using System.IO.Compression;
using WLEDAnimated.Interfaces;

namespace WLEDAnimated;

public class AnimationManager
{
    private readonly WLEDAnimationLoader _wledAnimationLoader;
    private readonly IImageSender _sender;
    private DirectoryInfo animationsFolder = null;

    public AnimationManager(WLEDAnimationLoader wledAnimationLoader, IImageSender sender)
    {
        _wledAnimationLoader = wledAnimationLoader;
        _sender = sender;
        var asmFile = new FileInfo(this.GetType().Assembly.Location);
        animationsFolder = new DirectoryInfo(System.IO.Path.Combine(asmFile.Directory.FullName, "Animations"));
        if (!animationsFolder.Exists) System.IO.Directory.CreateDirectory(animationsFolder.FullName);
    }

    public async Task<List<WLEDAnimation>> GetAnimations()
    {
        var animations = new List<WLEDAnimation>();
        foreach (var animationFolder in animationsFolder.GetDirectories())
        {
            var animation = await _wledAnimationLoader.LoadWLEDAnimation(animationFolder);
            animations.Add(animation);
        }
        return animations;
    }

    private DirectoryInfo GetAnimationFolderByAnimationName(string animationName)
    {
        return new DirectoryInfo(System.IO.Path.Combine(animationsFolder.FullName, animationName));
    }

    public async Task<IAnimation> UploadAndPlayAnimation(Stream fileStream, string fileName)
    {
        var animation = await UploadAnimation(fileStream, fileName);
        return await PlayAnimation(animation);
    }

    public async Task<IAnimation> UploadAnimation(Stream fileStream, string fileName)
    {
        var fileInfo = new FileInfo(fileName);
        IAnimation animation = null;

        if (fileInfo.Extension.EndsWith("wled", StringComparison.InvariantCultureIgnoreCase))
        {
            var zipRoot = new DirectoryInfo(System.IO.Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            if (!zipRoot.Exists)
                zipRoot.Create();

            var tempZipFile = System.IO.Path.Combine(zipRoot.FullName, fileName);

            using (var file = File.Create(tempZipFile))
            {
                await fileStream.CopyToAsync(file);
            }

            try
            {
                using (var zip = ZipFile.Open(tempZipFile, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.FullName.EndsWith("Animation.json", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var animationFile = new FileInfo(System.IO.Path.Combine(zipRoot.FullName, entry.FullName));
                            entry.ExtractToFile(animationFile.FullName);

                            //var loader = new WLEDAnimationLoader(_sender);
                            animation = await _wledAnimationLoader.LoadAnimation(animationFile.Directory);
                            var name = animation.Name;
                            var animationFolder = GetAnimationFolderByAnimationName(name);
                            System.IO.Compression.ZipFile.ExtractToDirectory(tempZipFile, animationFolder.FullName, overwriteFiles: true);
                            animation = await _wledAnimationLoader.LoadAnimation(animationFolder);
                            break;
                        }
                    }
                }
            }
            finally
            {
                System.IO.File.Delete(tempZipFile);
            }
        }
        return animation;
    }

    public async Task<IAnimation> PlayAnimation(IAnimation animation, object state = null)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var progress = new Progress<AnimationProgress>(p =>
        {
            Console.WriteLine($"Overall Progress: {p.OverallProgress}%");
            Console.WriteLine($"Current Transition: {p.CurrentTransition}, Progress: {p.TransitionProgress}%");
        });

        // Start the animation asynchronously
        await animation.StartAsync(cancellationTokenSource.Token, progress, state);
        return animation;
    }

    public async Task<IAnimation> PlayAnimation(string name, object state = null)
    {
        IAnimation animation = null;
        try
        {
            animation = await _wledAnimationLoader.LoadAnimation(new DirectoryInfo(System.IO.Path.Combine(animationsFolder.FullName, name)));
        }
        catch (Exception e)
        {
            animation = await _wledAnimationLoader.LoadAnimation(new DirectoryInfo(System.IO.Path.Combine(animationsFolder.FullName, $"{name} Animation")));
        }

        if (animation != null)
        {
            return await PlayAnimation(animation, state);
        }
        return null;
    }
}