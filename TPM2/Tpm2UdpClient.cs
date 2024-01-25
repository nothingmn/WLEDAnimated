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
    private const byte PacketStartByte = 0x9C; //156
    private const byte PacketEndByte = 0x36; //54
    private const PacketTypes PacketType = PacketTypes.DataFrame; // Assuming a standard packet type for LED data
    private UdpClient udpClient;
    private IPEndPoint endPoint;

    public void Connect(string ipAddress, int port)
    {
        udpClient = new UdpClient();
        endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }

    public void SendLEDStrip(LEDStrip strip)
    {
        var payload = ConstructPayload(strip);
        SendData(payload);
    }

    private void SendData(byte[] data)
    {
        udpClient.Send(data, data.Length - 1, endPoint);  //strip.show();

        for (int x = 0; x <= data.Length - 1; x++) Console.Write($"{data[x]} ");

        Console.WriteLine($"->{endPoint}");

        //var server = new TestServer();
        //server.TestPayload(data);
    }

    public byte[] ConstructPayload(LEDStrip strip)
    {
        var payload = ConvertStripToPayload(strip);
        var packet = new List<byte>()
        {
            PacketStartByte,  //(udpIn[0] == 0x9c)
            (byte)PacketTypes.DataFrame // byte tpmType = udpIn[1];       if (tpmType != 0xda) return; //return if notTPM2.NET data
        };
        packet.AddRange(payload);
        packet.Add(PacketEndByte);

        return packet.ToArray();
    }

    public void SendStrip(LEDStrip strip)
    {
        SendData(ConstructPayload(strip));
    }

    private byte[] ConvertStripToPayload(LEDStrip strip)
    {
        var leds = strip.GetLEDs();
        var frameSize = (leds.Length - 1) * 3;
        var frame = new List<byte>()
        {
            //    if (tpmPacketCount == 1) tpmPayloadFrameSize = (udpIn[2] << 8) + udpIn[3]; //save frame size for the whole payload if this is the first packet
            (byte)((frameSize >> 8) & 0xFF), //Frame size in 16 bits - High-Byte first, then
            (byte)(frameSize & 0xFF), // Low-Byte
            1, //packetNumber,     byte packetNum = udpIn[4]; //starts with 1!
            1  //packetCount,      byte numPackets = udpIn[5];     if (tpmPacketCount == numPackets) //reset packet count and show if all packets were received
        };
        //int reconstructedFrameSize = (frame[0] << 8) | frame[1];

        //uint16_t id = (tpmPayloadFrameSize / 3) * (packetNum - 1); //start LED
        //setRealtimePixel(id, udpIn[i], udpIn[i+1], udpIn[i+2], 0);
        foreach (var led in leds)
        {
            frame.Add((byte)led.R);
            frame.Add((byte)led.G);
            frame.Add((byte)led.B);
        }

        return frame.ToArray();
    }

    public void Dispose()
    {
        udpClient.Dispose();
    }
}