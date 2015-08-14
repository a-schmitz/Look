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

        public delegate void ScreenUpdateHandler(object sender, ScreenUpdateEventArgs e);
        public delegate void HostConnectedHandler(object sender, HostConnectedEventArgs e);

        public event ScreenUpdateHandler OnScreenUpdateReceived;
        public event HostConnectedHandler OnHostConnected;

        #endregion

        #region Public Methods

        public void StartAcceptingConnections(string name) {
            if (this.acceptingConnections)
                return;

            this.serviceHost.Start<ViewService>(name);
            ViewService.OnHostConnected += this.ViewServiceOnOnHostConnected;
            ViewService.OnImageChange += this.ViewServiceOnOnImageChange;

            this.acceptingConnections = true;
        }

        private void ViewServiceOnOnHostConnected(object sender, HostConnectedEventArgs e) {
            this.RaiseOnHostConnected(e);
            if (e.Accepted) {
                var address = new EndpointAddress(string.Format(ViewServiceHost.BASE_ADDRESS, e.Host, ViewServiceHost.PORT));
                var proxy = new ViewServiceClient(address);
                this.connectedHosts.TryAdd(e.Host.ToLower(), proxy);
            }
        }

        public void StopAcceptingConnections() {
            if (!this.acceptingConnections)
                return;

            this.serviceHost.Stop();
            ViewService.OnImageChange -= this.ViewServiceOnOnImageChange;

            this.acceptingConnections = false;
        }

        public void Dispose() {
            this.StopAcceptingConnections();
        }

        public IEnumerable<SharingEndpoint> FindClients() {
            using (var client = new ViewServiceClient()) {
                foreach (var endpoint in client.Discover()) {
#if !DEBUG
                    if (!this.IsLocal(endpoint.Address)) // enable local testing
#endif
                    yield return endpoint;
                }
            }
        }

        private bool IsLocal(EndpointAddress address) {
            // TODO suffiecient?
            return string.Compare(address.Uri.Host, Dns.GetHostName(), StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public async Task<bool> ConnectAsync(SharingEndpoint endpoint) {
            var host = endpoint.Address.Uri.Host.ToLower();
            if (this.connectedHosts.ContainsKey(host))
                return true;

            var proxy = new ViewServiceClient(endpoint.Address);

            var result = await Task.Factory.StartNew<bool>(proxy.Connect);
            if (result)
                this.connectedHosts.TryAdd(host, proxy);

            return result;
        }

        public bool Connect(SharingEndpoint endpoint) {
            return this.ConnectAsync(endpoint).Result;
        }

        public bool Connect(string host) {
            throw new NotImplementedException();
        }

        public void Disconnect(string host) {
            ViewServiceClient proxy;
            if (this.connectedHosts.TryRemove(host.ToLower(), out proxy)) {
                proxy.Disconnect();
                proxy.Close();
            }
        }

        public void ShareWindows(string host, List<Window> windows) {
            var proxy = this.GetProxy(host);
            proxy.PushWindowList(windows);
        }

        public void RequestWindowTransfer(string host, List<Window> windows)
        {
            var proxy = this.GetProxy(host);
            proxy.RequestWindowTransfer(windows);
        }
        
        #endregion

        #region Private Methods

        private void RaiseOnScreenUpdateReceived(ScreenUpdateEventArgs e) {
            if (this.OnScreenUpdateReceived != null) {
                this.OnScreenUpdateReceived(this, e);
            }
        }

        private void RaiseOnHostConnected(HostConnectedEventArgs e) {
            if (this.OnHostConnected != null) {
                this.OnHostConnected(this, e);
            }
        }

        private void ViewServiceOnOnImageChange(Image display, string id, string host) {
            this.RaiseOnScreenUpdateReceived(new ScreenUpdateEventArgs { Host = host, WindowId = id, Screen = display });
        }

        #endregion

        public ViewServiceClient GetProxy(string host) {
            ViewServiceClient proxy;
            if (this.connectedHosts.TryGetValue(host.ToLower(), out proxy)) {
                return proxy;
            }

            throw new HostNotConnectedException();
        }
    }
}