using SixLabors.ImageSharp;
using System.Net.Sockets;
using System.Xml;
using WLEDAnimated;
using Color = System.Drawing.Color;

namespace WLEDAnimateConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        // IP Address and Port of the WLED device
        string ipAddress = "10.0.0.217"; // Replace with the correct IP
        int port = 21324; // Default port for WLED UDP

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