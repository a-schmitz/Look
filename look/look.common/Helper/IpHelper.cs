namespace look.common.Helper
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    public static class IpHelper
    {
        /// <summary>
        /// Gets the local IPv4 address, fallback FQDN
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp() {
            var localIp = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            
            if (localIp != null) {
                return localIp.ToString();
            }

            var domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            var hostName = Dns.GetHostName();

            if (!hostName.EndsWith(domainName)) {
                hostName += "." + domainName;
            }

            return hostName;
        }

        /// <summary>
        /// Gets the host's IPv4 address as string representation
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static string GetIp(string host) {

            // enable local testing
            IPAddress address;
            if (IPAddress.TryParse(host, out address))
                if (IPAddress.IsLoopback(address))
                    return GetLocalIp();

            var localIp = Dns.GetHostAddresses(host).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            return localIp != null ? localIp.ToString() : null;
        }

        public static Uri GetMulticastAddress()
        {
            // TODO port configurable default 3702
            if (!Socket.OSSupportsIPv4)
            {
                return new Uri("soap.udp://[FF02::C]:10001");
            }
            return new Uri("soap.udp://239.255.255.250:10001");
        }
    }
}
