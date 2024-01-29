using System;
using System.Net;
using System.Net.Sockets;

namespace WLEDAnimated;

public class EndpointConverter
{
    public IPEndPoint GetIPEndPoint(string host, int port)
    {
        IPAddress ipAddress;
        if (!IPAddress.TryParse(host, out ipAddress))
        {
            try
            {
                // This will get both IPv4 and IPv6 addresses for the host name
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                if (addresses.Length == 0)
                {
                    throw new ArgumentException("Unable to resolve host to an IP address.");
                }

                // You might want to select either an IPv4 or IPv6 address explicitly here,
                // depending on your specific requirements.
                ipAddress = addresses[0]; // Taking the first IP address (could be IPv4 or IPv6)
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid host name or IP address.", ex);
            }
        }

        return new IPEndPoint(ipAddress, port);
    }
}