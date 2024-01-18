using System.Net;
using System.Xml;

namespace TPM2;

public class TestServer
{
    public void SendTPM2Ack()
    {
        Console.WriteLine("SendTPM2Ack");
    }

    public void RealtimeLock(int realtimeTimeoutMs, int realtimeModeTpm2net)
    {
        Console.WriteLine($"RealtimeLock {realtimeTimeoutMs} {realtimeModeTpm2net}");
    }

    public void SetRealtimePixel(int id, byte r, byte g, byte b, byte w)
    {
        Console.WriteLine($"SetRealtimePixel {id} {r} {g} {b} {w}");
    }

    public void Show()
    {
        Console.WriteLine("Show");
    }

    public void TestPayload(byte[] udpIn)
    {
        var tpmPacketCount = 0;
        var tpmPayloadFrameSize = 0;
        if (udpIn[0] == 0x9c)
        {
            // Handling TPM2.NET protocol
            byte tpmType = udpIn[1];
            if (tpmType == 0xaa)
            {
                SendTPM2Ack(); // Send acknowledgment for TPM2.NET polling
                return;
            }
            if (tpmType != 0xda) return; // Return if it's not TPM2.NET data

            // Get the IP address from UDP source, adapt this as per your network handling
            //IPAddress realtimeIP = /* Your method to get the remote IP address */;

            // Lock and timeout handling, adapt as per your application logic
            RealtimeLock(0, 0);

            tpmPacketCount++; // Increment packet count
            if (tpmPacketCount == 1)
            {
                tpmPayloadFrameSize = (ushort)((udpIn[2] << 8) + udpIn[3]); // Calculate frame size for the payload
            }
            byte packetNum = udpIn[4]; // Starts with 1!
            byte numPackets = udpIn[5];

            int id = (tpmPayloadFrameSize / 3) * (packetNum - 1); // Start LED
            int totalLen = 8 * 32; /* Get total length of your LED strip */;
            for (int i = 6; i < tpmPayloadFrameSize + 4; i += 3)
            {
                if (id < totalLen)
                {
                    SetRealtimePixel(id, udpIn[i], udpIn[i + 1], udpIn[i + 2], 0);
                    id++;
                }
                else break;
            }
            if (tpmPacketCount == numPackets) // Reset packet count and display if all packets were received
            {
                tpmPacketCount = 0;
                // Display the strip, adapt as per your LED control logic
                Show();
            }
        }
    }
}