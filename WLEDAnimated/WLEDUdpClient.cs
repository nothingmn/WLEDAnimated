using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WLEDAnimated;

//Value	Description	Max. LEDs
// 1	WARLS	255
// 2	DRGB	490
// 3	DRGBW	367
// 4	DNRGB	489/packet
// 0	WLED Notifier	-
public enum Protocols
{
    WLED = 0,
    WARLS = 1,
    DRGB = 2,
    DRGBW = 3,
    DNRGB = 4
}

public class WLEDUdpClient
{
    private readonly string _host;
    private readonly int _port;

    public Protocols Protocols { get; set; } = Protocols.DNRGB;
    public int Timeout { get; set; } = 1;

    public WLEDUdpClient(string host = "10.0.0.217", int port = 21324)
    {
        _host = host;
        _port = port;
    }

    public void Send(List<Frame> frames)
    {
        var converter = new DNRGBFrameConverter();
        using (var client = new System.Net.Sockets.UdpClient(_host, _port))
        {
            int x = 0;
            var data = converter.ConvertFrame(frames[0]);
            foreach (var payload in data)
            {
                client.Send(payload, payload.Length);
                Console.WriteLine($"Frame:{x} sent {payload.Length} bytes total");
                x++;
            }
        }
    }
}

public class Frame
{
    private readonly int _width;
    private readonly int _height;
    public Dictionary<int, Color> Pixels { get; set; } = new Dictionary<int, Color>();

    public Frame(int width = 32, int height = 16, Dictionary<int, Color> pixels = null)
    {
        _width = width;
        _height = height;
        if (pixels != null) Pixels = pixels;
    }
}

public class DNRGBFrameConverter
{
    public List<byte[]> ConvertFrame(Frame frame)
    {
        var data = new List<byte[]>();
        data.Add(new byte[] { Convert.ToByte((int)Protocols.DNRGB), Convert.ToByte('1') });

        int led = 0;
        foreach (var pixel in frame.Pixels)
        {
            var payload = new byte[]
            {
                (byte)led,
                pixel.Value.R,
                pixel.Value.G,
                pixel.Value.B
            };
            data.Add(payload);
        }

        return data;
    }
}