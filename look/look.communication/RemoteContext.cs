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

        public event HostConnectedHandler OnHostConnected;
        public event WindowsSharedHandler OnWindowsShared;
        public event WindowsRequestedHandler OnWindowsRequested;
        public event ScreenUpdateHandler OnScreenUpdateReceived;
        public event HostDisconnectedHandler OnHostDisconnected;

        #endregion

        #region Public Methods

        public void StartAcceptingConnections(string name) {
            if (this.acceptingConnections)
                return;

            this.serviceHost.Start<ViewService>(name);
            ViewService.OnHostConnected += this.ViewServiceOnOnHostConnected;
            ViewService.OnWindowsShared += this.ViewServiceOnOnWindowsShared;
            ViewService.OnWindowsRequested += this.ViewServiceOnOnWindowsRequested;
            ViewService.OnImageChange += this.ViewServiceOnOnImageChange;
            ViewService.OnHostDisconnected += this.ViewServiceOnOnHostDisconnected;

            this.acceptingConnections = true;
        }        

        public void StopAcceptingConnections() {
            if (!this.acceptingConnections)
                return;

            this.serviceHost.Stop();
            ViewService.OnHostConnected -= this.ViewServiceOnOnHostConnected;
            ViewService.OnWindowsShared -= this.ViewServiceOnOnWindowsShared;
            ViewService.OnWindowsRequested -= this.ViewServiceOnOnWindowsRequested;
            ViewService.OnImageChange -= this.ViewServiceOnOnImageChange;
            ViewService.OnHostDisconnected -= this.ViewServiceOnOnHostDisconnected;

            this.acceptingConnections = false;
        }

        public void Dispose() {
            this.StopAcceptingConnections();
        }

        public IEnumerable<RemoteHost> FindClients()
        {
            using (var client = new ViewServiceClient()) {
                foreach (var endpoint in client.Discover()) {
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
                    if (!endpoint.Address.IsLoopback) // enable local testing
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
            var address = new EndpointAddress(string.Format(ViewServiceHost.BASE_ADDRESS, IpHelper.GetIp(hostOrIp), ViewServiceHost.PORT));
            var endpoint = new SharingEndpoint("<quickadd>", address);

            return this.ConnectAsync(endpoint).Result;
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
        
        #endregion

        #region Private Methods

        private void RaiseOnHostConnected(HostConnectedEventArgs e)
        {
            if (this.OnHostConnected != null)
            {
                this.OnHostConnected(this, e);
            }
        }

        private void RaiseOnWindowsShared(WindowsSharedEventArgs e)
        {
            if (this.OnWindowsShared != null)
            {
                this.OnWindowsShared(this, e);
            }
        }

        private void RaiseOnWindowsRequested(WindowsRequestedEventArgs e)
        {
            if (this.OnWindowsRequested != null)
            {
                this.OnWindowsRequested(this, e);
            }
        }

        private void RaiseOnScreenUpdateReceived(ScreenUpdateEventArgs e)
        {
            if (this.OnScreenUpdateReceived != null)
            {
                this.OnScreenUpdateReceived(this, e);
            }
        }

        private void RaiseOnHostDisconnected(HostDisconnectedEventArgs e)
        {
            if (this.OnHostDisconnected != null)
            {
                this.OnHostDisconnected(this, e);
            }
        }
        

        private void ViewServiceOnOnImageChange(Image display, string id, string ip) {
            this.RaiseOnScreenUpdateReceived(new ScreenUpdateEventArgs { Ip = ip, WindowId = id, Screen = display });
        }

        private void ViewServiceOnOnHostConnected(object sender, HostConnectedEventArgs e)
        {
            this.RaiseOnHostConnected(e);
            if (e.Accepted)
            {
                var address = new EndpointAddress(string.Format(ViewServiceHost.BASE_ADDRESS, e.Ip, ViewServiceHost.PORT));
                var proxy = new ViewServiceClient(address);
                this.connectedHosts.TryAdd(e.Ip, proxy);
            }
        }

        private void ViewServiceOnOnWindowsShared(object sender, WindowsSharedEventArgs e)
        {
            this.RaiseOnWindowsShared(e);
        }

        private void ViewServiceOnOnWindowsRequested(object sender, WindowsRequestedEventArgs e)
        {
            this.RaiseOnWindowsRequested(e);
        }

        private void ViewServiceOnOnHostDisconnected(object sender, HostDisconnectedEventArgs e)
        {
            this.RaiseOnHostDisconnected(e);
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