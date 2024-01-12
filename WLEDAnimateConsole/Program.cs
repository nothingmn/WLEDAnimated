using SixLabors.ImageSharp;
using System.Net.Sockets;
using System.Xml;
using WLEDAnimated;
using Color = System.Drawing.Color;

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

        await APIBasicOnOffBrightness(ipAddress);

        var apiManager = new WLEDApiManager(ipAddress);
        await apiManager.Connect();
        await apiManager.On(10);
        await apiManager.ScrollingText(DateTime.UtcNow.ToLongDateString());
    }

    private static async Task APIBasicOnOffBrightness(string ipAddress)
    {
        var apiManager = new WLEDApiManager(ipAddress);
        await apiManager.Connect();
        await apiManager.On(1);

        for (int x = 1; x <= 30; x += 10)
        {
            await apiManager.SetBrightness(x);
            await Task.Delay(500);
        }
    }

    private static void UDPSender(string ipAddress, int port)
    {
        var sender = new ImageUDPSender();
        var img = "ghosty.gif";
        //img = "nyancat.gif";
        //img = "red-8x32.png";
        img = "ghosty-8x32.gif";

        sender.Send(
            ipAddress,
            port,
            img,
            new Size(32, 8),
            0,
            2,
            100,
            10
        );
    }
}