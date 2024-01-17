using AnimationCore.Interfaces;
using AnimationCore;
using System.Runtime.CompilerServices;
using WLEDAnimated.Animation;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json.Converters;

namespace WLEDAnimated;

public class AnimationManager
{
    private readonly WLEDAnimationLoader _wledAnimationLoader;
    private DirectoryInfo animationsFolder = null;

    public AnimationManager(WLEDAnimationLoader wledAnimationLoader)
    {
        _wledAnimationLoader = wledAnimationLoader;
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

                            var loader = new WLEDAnimationLoader();
                            animation = await loader.LoadAnimation(animationFile.Directory);
                            var name = animation.Name;
                            var animationFolder = GetAnimationFolderByAnimationName(name);
                            System.IO.Compression.ZipFile.ExtractToDirectory(tempZipFile, animationFolder.FullName, overwriteFiles: true);
                            animation = await loader.LoadAnimation(animationFolder);
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

    public async Task<IAnimation> PlayAnimation(IAnimation animation)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var progress = new Progress<AnimationProgress>(p =>
        {
            Console.WriteLine($"Overall Progress: {p.OverallProgress}%");
            Console.WriteLine($"Current Transition: {p.CurrentTransition}, Progress: {p.TransitionProgress}%");
        });

        // Start the animation asynchronously
        await animation.StartAsync(cancellationTokenSource.Token, progress);
        return animation;
    }

    public async Task<IAnimation> PlayAnimation(string name)
    {
        var loader = new WLEDAnimationLoader();
        var animation = await loader.LoadAnimation(new DirectoryInfo(System.IO.Path.Combine(animationsFolder.FullName, name)));
        return await PlayAnimation(animation);
    }
}