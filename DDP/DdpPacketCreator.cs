using System;
using System.Collections.Generic;
using System.Linq;

namespace DDP;

public class DdpPacketCreator
{
    private byte sequenceId;
    private readonly byte DDP_DESTINATION_ID = 0x01; // Adjust as necessary
    private const int HEADER_SIZE = 10;
    private const int MAX_PACKET_SIZE = 1000; // Including header
    private const int MAX_DATA_SIZE = MAX_PACKET_SIZE - HEADER_SIZE;

    public DdpPacketCreator(byte initialSequenceId = 0)
    {
        this.sequenceId = initialSequenceId;
    }

    public List<byte[]> CreateDdpPackets(byte[] data)
    {
        List<byte[]> packets = new List<byte[]>();
        int totalLength = data.Length;
        int offset = 0;

        while (offset < totalLength)
        {
            bool isLastPacket = (totalLength - offset) <= MAX_DATA_SIZE;
            int packetDataSize = isLastPacket ? totalLength - offset : MAX_DATA_SIZE;
            byte[] packetData = new byte[packetDataSize];
            Array.Copy(data, offset, packetData, 0, packetDataSize);

            // Create packet header
            byte[] header = new byte[HEADER_SIZE];
            header[0] = (byte)(0b01000000 | (isLastPacket ? 0b00000001 : 0));
            header[1] = this.sequenceId;
            header[2] = 0x01; // Data type set to 01
            header[3] = this.DDP_DESTINATION_ID;
            Array.Copy(BitConverter.GetBytes(offset).Reverse().ToArray(), 0, header, 4, 4); // Data offset
            Array.Copy(BitConverter.GetBytes(packetDataSize).Reverse().ToArray(), 0, header, 8, 2); // Data length

            // Combine header and packet data into a single packet
            byte[] packet = new byte[MAX_PACKET_SIZE];
            Array.Copy(header, 0, packet, 0, HEADER_SIZE);
            Array.Copy(packetData, 0, packet, HEADER_SIZE, packetDataSize);

            // Pad the end of the packet with zeros if it's the last packet and not full
            if (isLastPacket && packetDataSize < MAX_DATA_SIZE)
            {
                for (int i = HEADER_SIZE + packetDataSize; i < packet.Length; i++)
                {
                    packet[i] = 0;
                }
            }

            packets.Add(packet);

            offset += MAX_DATA_SIZE;

            // Increment sequence ID with wrapping
            sequenceId = (byte)((sequenceId + 1) % 256);
        }

        return packets;
    }
}