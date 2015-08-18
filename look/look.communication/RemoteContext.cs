namespace look.communication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Net;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using look.common.Events;
    using look.common.Exceptions;
    using look.common.Helper;
    using look.common.Model;
    using look.communication.Clients;
    using look.communication.Hosts;
    using look.communication.Model;
    using look.communication.Services;

    public sealed class RemoteContext : IDisposable
    {
        #region fields

        private static readonly object SyncRoot = new object();

        private static volatile RemoteContext instance;

        private readonly ViewServiceHost serviceHost;

        private bool acceptingConnections;

        private readonly ConcurrentDictionary<string, ViewServiceClient> connectedHosts;

        #endregion

        #region public properties

        public static RemoteContext Instance {
            get {
                if (instance == null) {
                    lock (SyncRoot) {
                        if (instance == null)
                            instance = new RemoteContext();
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Constructors

        private RemoteContext() {
            this.serviceHost = new ViewServiceHost();
            this.acceptingConnections = false;
            this.connectedHosts = new ConcurrentDictionary<string, ViewServiceClient>();
        }

        #endregion

        #region Events

        public delegate void HostConnectedHandler(object sender, HostConnectedEventArgs e);
        public delegate void WindowsSharedHandler(object sender, WindowsSharedEventArgs e);
        public delegate void WindowsRequestedHandler(object sender, WindowsRequestedEventArgs e);
        public delegate void ScreenUpdateHandler(object sender, ScreenUpdateEventArgs e);
        public delegate void HostDisconnectedHandler(object sender, HostDisconnectedEventArgs e);

        public event HostConnectedHandler HostConnected;
        public event WindowsSharedHandler WindowsShared;
        public event WindowsRequestedHandler WindowsRequested;
        public event ScreenUpdateHandler ScreenUpdateReceived;
        public event HostDisconnectedHandler HostDisconnected;

        #endregion

        #region Public Methods

        public void StartAcceptingConnections(string name) {
            if (this.acceptingConnections)
                return;

            this.serviceHost.Start<ViewService>(name);
            ViewService.HostConnected += this.ViewServiceHostConnected;
            ViewService.WindowsShared += this.ViewServiceWindowsShared;
            ViewService.WindowsRequested += this.ViewServiceWindowsRequested;
            ViewService.ImageChanged += this.ViewServiceImageChanged;
            ViewService.HostDisconnected += this.ViewServiceHostDisconnected;

            this.acceptingConnections = true;
        }        

        public void StopAcceptingConnections() {
            if (!this.acceptingConnections)
                return;

            this.serviceHost.Stop();
            ViewService.HostConnected -= this.ViewServiceHostConnected;
            ViewService.WindowsShared -= this.ViewServiceWindowsShared;
            ViewService.WindowsRequested -= this.ViewServiceWindowsRequested;
            ViewService.ImageChanged -= this.ViewServiceImageChanged;
            ViewService.HostDisconnected -= this.ViewServiceHostDisconnected;

            this.acceptingConnections = false;
        }

        public void Dispose() {
            this.StopAcceptingConnections();
        }

        public IEnumerable<RemoteHost> FindClients()
        {
            using (var client = new ViewServiceClient())
            {
                foreach (var endpoint in client.Discover())
                {
#if !DEBUG
                    if (!endpoint.Address.IsLoopback) // enable local testing
#endif
                    yield return new RemoteHost { Ip = IpHelper.GetIp(endpoint.Address.Uri.Host), Name = endpoint.Name };
                }
            }
        }

        public async Task<IEnumerable<RemoteHost>> FindClientsAsync()
        {
            var hosts = new List<RemoteHost>();
            using (var client = new ViewServiceClient())
            {
                foreach (var endpoint in await client.DiscoverAsync())
                {
#if !DEBUG
                    if (!endpoint.Address.Uri.IsLoopback) // enable local testing
#endif
                    hosts.Add(new RemoteHost { Ip = IpHelper.GetIp(endpoint.Address.Uri.Host), Name = endpoint.Name });
                }
            }
            return hosts;
        }

        public async Task<bool> ConnectAsync(SharingEndpoint endpoint) {
            var ip = IpHelper.GetIp(endpoint.Address.Uri.Host);
            if (this.connectedHosts.ContainsKey(ip))
                return true;

            var proxy = new ViewServiceClient(endpoint.Address);
            var result = await Task.Factory.StartNew<bool>(proxy.Connect);
            if (result)
                this.connectedHosts.TryAdd(ip, proxy);

            return result;
        }

        public bool Connect(SharingEndpoint endpoint) {
            return this.ConnectAsync(endpoint).Result;
        }

        public bool Connect(string hostOrIp) {
            return this.ConnectAsync(hostOrIp).Result;
        }

        public async Task<bool> ConnectAsync(string hostOrIp) {
            var address = new EndpointAddress(string.Format(ViewServiceHost.BASE_ADDRESS, IpHelper.GetIp(hostOrIp), ViewServiceHost.PORT));
            var endpoint = new SharingEndpoint("<quickadd>", address);

            return await this.ConnectAsync(endpoint);
        }

        public void Disconnect(string hostOrIp)
        {
            ViewServiceClient proxy;
            if (this.connectedHosts.TryRemove(IpHelper.GetIp(hostOrIp), out proxy)) {
                proxy.Disconnect();                
            }
        }

        public void ShareWindows(string hostOrIp, List<Window> windows)
        {
            var proxy = this.GetProxy(hostOrIp);
            proxy.PushWindowList(windows);
        }

        public void RequestWindowTransfer(string hostOrIp, List<Window> windows)
        {
            var proxy = this.GetProxy(hostOrIp);
            proxy.RequestWindowTransfer(windows);
        }

        public bool IsConnected(string hostOrIp) {
            return this.connectedHosts.ContainsKey(IpHelper.GetIp(hostOrIp));
        }

        #endregion

        #region Private Methods

        private void OnHostConnected(HostConnectedEventArgs e)
        {
            if (this.HostConnected != null)
            {
                this.HostConnected(this, e);
            }
        }

        private void OnWindowsShared(WindowsSharedEventArgs e)
        {
            if (this.WindowsShared != null)
            {
                this.WindowsShared(this, e);
            }
        }

        private void OnWindowsRequested(WindowsRequestedEventArgs e)
        {
            if (this.WindowsRequested != null)
            {
                this.WindowsRequested(this, e);
            }
        }

        private void OnScreenUpdateReceived(ScreenUpdateEventArgs e)
        {
            if (this.ScreenUpdateReceived != null)
            {
                this.ScreenUpdateReceived(this, e);
            }
        }

        private void OnHostDisconnected(HostDisconnectedEventArgs e)
        {
            if (this.HostDisconnected != null)
            {
                this.HostDisconnected(this, e);
            }
        }
        

        private void ViewServiceImageChanged(Image display, string id, string ip) {
            this.OnScreenUpdateReceived(new ScreenUpdateEventArgs { Ip = ip, WindowId = id, Screen = display });
        }

        private void ViewServiceHostConnected(object sender, HostConnectedEventArgs e)
        {
            this.OnHostConnected(e);
            if (e.Accepted)
            {
                var address = new EndpointAddress(string.Format(ViewServiceHost.BASE_ADDRESS, e.Ip, ViewServiceHost.PORT));
                var proxy = new ViewServiceClient(address);
                this.connectedHosts.TryAdd(e.Ip, proxy);
            }
        }

        private void ViewServiceWindowsShared(object sender, WindowsSharedEventArgs e)
        {
            this.OnWindowsShared(e);
        }

        private void ViewServiceWindowsRequested(object sender, WindowsRequestedEventArgs e)
        {
            this.OnWindowsRequested(e);
        }

        private void ViewServiceHostDisconnected(object sender, HostDisconnectedEventArgs e)
        {
            this.OnHostDisconnected(e);
            ViewServiceClient proxy;
            this.connectedHosts.TryRemove(IpHelper.GetIp(e.Ip), out proxy);
        }

        #endregion

        public ViewServiceClient GetProxy(string hostOrIp)
        {
            ViewServiceClient proxy;
            if (this.connectedHosts.TryGetValue(IpHelper.GetIp(hostOrIp), out proxy))
            {
                return proxy;
            }

            throw new HostNotConnectedException();
        }
    }
}