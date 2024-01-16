using SixLabors.ImageSharp;
using System.Net.Sockets;
using System.Xml;
using AnimationCore;
using WLEDAnimated;
using Color = System.Drawing.Color;
using AnimationCore.Interfaces;
using Newtonsoft.Json;
using WLEDAnimated.Animation;
using System.Threading;
using System;

namespace WLEDAnimateConsole;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // IP Address and Port of the WLED device
        string ipAddress = "10.0.0.217"; // Replace with the correct IP
        int port = 21324; // Default port for WLED UDP
        System.AppDomain.CurrentDomain.UnhandledException += (sender, e) => Console.WriteLine(e.ExceptionObject.ToString());

        //UDPSender(ipAddress, port);
        //await ApiManagerTesting(ipAddress);

        //await AnimationTest(ipAddress, port);

        //await WLEDAnimationTest();
        await WLEDAnimationFromWLEDFile();
    }

    public static async Task WLEDAnimationFromWLEDFile()
    {
        var animationManager = new AnimationManager(new WLEDAnimationLoader());
        await animationManager.UploadAnimation(new FileInfo("SampleAnimation/SampleAnimation.wled").OpenRead(), "SampleAnimation.wled");

        await animationManager.UploadAndPlayAnimation(new FileInfo("SampleAnimation/SampleAnimation.wled").OpenRead(), "SampleAnimation.wled");
        await animationManager.PlayAnimation("Animation Sample");
    }

    public static async Task WLEDAnimationTest()
    {
        var loader = new WLEDAnimationLoader();
        var animation = await loader.LoadAnimation(new DirectoryInfo("SampleAnimation"));

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
    }

    public static async Task AnimationTest(string ipAddress, int port)
    {
        var step1 = new SimpleStep
        {
            Description = "Step 1",
            Transition = async (cancellationToken) =>
            {
                Console.WriteLine("Step 1: Step 1 Finished, moving to Step 2");
                UDPSender(ipAddress, port, "blue-8x32.png");
            }
        };
        var step2 = new SimpleStep
        {
            Description = "Step 2",
            Transition = async (cancellationToken) =>
            {
                Console.WriteLine("Step 2 started transitioning");
                UDPSender(ipAddress, port, "green-8x32.png");
                Console.WriteLine("Step 2 transitioned");
            }
        };
        var step3 = new SimpleStep
        {
            Description = "Step 3",
            Transition = async (cancellationToken) =>
            {
                Console.WriteLine("Step 3: Step 2 Finished, moving to Step 3");
                UDPSender(ipAddress, port, "red-8x32.png");
            }
        };
        var transition = new BasicTransition
        {
            ToStep = step2,
            Duration = TimeSpan.FromSeconds(2),
            StartDelay = TimeSpan.FromSeconds(1),
        };
        var transition1 = new BasicTransition
        {
            ToStep = step3,
            Duration = TimeSpan.FromSeconds(2),
            StartDelay = TimeSpan.FromSeconds(1),
        };

        transition.TransitionStarted += (sender, e) => Console.WriteLine("Transition Started");
        transition.TransitionCompleted += (sender, e) => Console.WriteLine("Transition Completed");
        transition1.TransitionStarted += (sender, e) => Console.WriteLine("Transition1 Started");
        transition1.TransitionCompleted += (sender, e) => Console.WriteLine("Transition1 Completed");

        var animation = new LinearAnimation
        {
            Name = "Simple Animation",
            Duration = TimeSpan.FromSeconds(5),
            Transitions = new List<ITransition> { transition, transition1 }
        };

        animation.Started += (sender, e) => Console.WriteLine("Animation Started");
        animation.Stopped += (sender, e) => Console.WriteLine("Animation Stopped");
        animation.Completed += (sender, e) => Console.WriteLine("Animation Completed");

        var cancellationTokenSource = new CancellationTokenSource();
        var progress = new Progress<AnimationProgress>(p =>
        {
            Console.WriteLine($"Overall Progress: {p.OverallProgress}%");
            Console.WriteLine($"Current Transition: {p.CurrentTransition}, Progress: {p.TransitionProgress}%");
        });

        animation.ScheduleStart(DateTimeOffset.Now.AddSeconds(5));
        animation.ScheduleStop(DateTimeOffset.Now.AddSeconds(10));

        try
        {
            // Start the animation asynchronously
            await animation.StartAsync(cancellationTokenSource.Token, progress);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Animation was canceled.");
        }
    }

    private static async Task ApiManagerTesting(string ipAddress)
    {
        await APIBasicOnOffBrightness(ipAddress);

        var apiManager = new WLEDApiManager();
        await apiManager.Connect(ipAddress);
        await apiManager.On(10);
        await apiManager.ScrollingText(DateTime.UtcNow.ToLongDateString());
    }

    private static async Task APIBasicOnOffBrightness(string ipAddress)
    {
        var apiManager = new WLEDApiManager();
        await apiManager.Connect(ipAddress);
        await apiManager.On(1);

        for (int x = 1; x <= 30; x += 10)
        {
            await apiManager.SetBrightness(x);
            await Task.Delay(500);
        }
    }

    private static void UDPSender(string ipAddress, int port, string anmiation = "ghosty.gif")
    {
        var sender = new ImageUDPSender();

        sender.Send(
            ipAddress,
            port,
            anmiation,
            new Size(32, 8),
            0,
            2,
            100,
            10
        );
    }
}