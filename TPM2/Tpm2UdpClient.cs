using System.Net.Sockets;
using System.Net;
using System;

namespace TPM2;

/// <summary>
/// https://gist.github.com/jblang/89e24e2655be6c463c56
/// https://www.ledstyles.de/index.php?thread/18969-tpm2-protokoll-zur-matrix-lichtsteuerung/
///
/// https://github.com/Aircoookie/WLED/blob/main/wled00/udp.cpp#L473
/// </summary>
public enum PacketTypes : byte
{
    DataFrame = 0xDA, //218 = Data frame or
    Command = 0xC0,   //192= Command or
    RequestedResponse = 0xAA //170 = Requested response (from the data receiver to the sender)
}

public class Tpm2UdpClient : IDisposable
{
    private const byte PacketStartByte = 0x9C;
    private const byte PacketEndByte = 0x36;
    private const PacketTypes PacketType = PacketTypes.DataFrame;
    private const int MaxPayloadSize = 1380; // Maximum payload size
    private UdpClient udpClient;
    private IPEndPoint endPoint;

    public void Connect(string ipAddress, int port)
    {
        udpClient = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }

    public List<byte[]> ConstructPayload(LEDStrip strip)
    {
        var leds = strip.GetLEDs();
        var packets = new List<byte[]>();
        int maxLedsPerPacket = (MaxPayloadSize - 6) / 3; // 6 bytes for start byte, packet type, frame size (2 bytes), packet number, packet count, end byte

        for (int i = 0; i < leds.Length; i += maxLedsPerPacket)
        {
            var frame = new List<byte>
                {
                    PacketStartByte,
                    (byte)PacketType,
                    0, // Placeholder for frame size (high byte)
                    0, // Placeholder for frame size (low byte)
                    1, // packetNumber
                    1  // packetCount - Placeholder
                };

            int frameSize = 0;
            for (int j = i; j < Math.Min(i + maxLedsPerPacket, leds.Length); j++)
            {
                frame.Add((byte)leds[j].R);
                frame.Add((byte)leds[j].G);
                frame.Add((byte)leds[j].B);
                frameSize += 3;
            }

            frame[2] = (byte)((frameSize >> 8) & 0xFF);
            frame[3] = (byte)(frameSize & 0xFF);
            frame.Add(PacketEndByte);

            packets.Add(frame.ToArray());
        }

        // Update packet counts
        for (int k = 0; k < packets.Count; k++)
        {
            packets[k][5] = (byte)packets.Count;
            packets[k][4] = (byte)(k + 1);
        }

        return packets;
    }

    public void SendLEDStrip(LEDStrip strip)
    {
        var packets = ConstructPayload(strip);
        foreach (var packet in packets)
        {
            SendData(packet);
        }
    }

    private void SendData(byte[] data)
    {
        udpClient.Send(data, data.Length, endPoint);

        for (int x = 0; x < data.Length; x++) Console.Write($"{data[x]} ");
        Console.WriteLine($"->{endPoint}");
    }

    public void Dispose()
    {
        udpClient.Dispose();
    }
}