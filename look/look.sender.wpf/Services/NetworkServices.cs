// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkServices.cs" company="">
//   
// </copyright>
// <summary>
//   The network services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Services
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using look.common.Model;
    using look.communication;

    #endregion

    /// <summary>
    ///     The network services.
    /// </summary>
    public class NetworkServices
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get hostname by ip.
        /// </summary>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetHostnameByIp(string ip) {
            IPAddress address;
            if (!IPAddress.TryParse(ip, out address))
                throw new ArgumentException("IPAddress not valid!");

            var entry = Dns.GetHostEntry(address);
            return entry == null ? string.Empty : entry.HostName.Substring(0, entry.HostName.IndexOf("."));
        }

        /// <summary>
        /// The get ip by hostname.
        /// </summary>
        /// <param name="hostname">
        /// The hostname.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public string GetIpByHostname(string hostname) {
            if (hostname == null)
                throw new ArgumentNullException("hostname");

            var entry = Dns.GetHostEntry(hostname);
            if (entry == null || entry.AddressList.Length == 0)
                return string.Empty;
            return entry.AddressList[0].ToString();
        }

        /// <summary>
        /// The is valid ip address.
        /// </summary>
        /// <param name="ip">
        /// The ip.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValidIpAddress(string ip) {
            IPAddress address;
            return IPAddress.TryParse(ip, out address);
        }

        #endregion
    }

}