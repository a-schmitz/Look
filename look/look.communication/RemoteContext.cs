namespace look.communication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;

    using look.common.Events;
    using look.communication.Clients;
    using look.communication.Hosts;
    using look.communication.Model;
    using look.communication.Services;

    public sealed class RemoteContext : IDisposable
    {
        #region fields

        private static readonly object SyncRoot = new object();

        private static volatile RemoteContext instance;

        private readonly ViewServiceHost host;

        private bool acceptingConnections;

        private readonly ConcurrentDictionary<string, ViewServiceClient> connectedHosts; 

        #endregion

        #region public properties

        public static RemoteContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                            instance = new RemoteContext();
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Constructors

        private RemoteContext()
        {
            this.host = new ViewServiceHost();
            this.acceptingConnections = false;
            this.connectedHosts = new ConcurrentDictionary<string, ViewServiceClient>();
        }

        #endregion

        #region Events

        public delegate void ScreenUpdateHandler(object sender, ScreenUpdateEventArgs e);
        public event ScreenUpdateHandler OnScreenUpdateReceived;

        #endregion

        #region Public Methods

        public void StartAcceptingConnections(string name)
        {
            if (this.acceptingConnections)
                return;

            var threadScreen = new Thread(ThreadCOnnect);
            threadScreen.Start();

            this.host.Start<ViewService>(name);
            ViewService.OnImageChange += this.ViewServiceOnOnImageChange;

            this.acceptingConnections = true;
        }

        public void StopAcceptingConnections()
        {
            if (!this.acceptingConnections)
                return;
            
            this.host.Stop();
            ViewService.OnImageChange -= this.ViewServiceOnOnImageChange;
            
            this.acceptingConnections = false;
        }

        private void ThreadCOnnect()
        {
            this.host.Start<ViewService>("Kekse");
        }

        public void Dispose()
        {
            this.StopAcceptingConnections();
        }

        public IEnumerable<SharingEndpoint> FindClients()
        {
            using (var client = new ViewServiceClient())
            {
                return client.Discover();
            }
        }

        public async Task<bool> ConnectAsync(SharingEndpoint endpoint)
        {
            var host = endpoint.Address.Uri.Host.ToLower();
            if (this.connectedHosts.ContainsKey(host))
                return true;

            var ctx = new InstanceContext(new ViewService());
            var proxy = new ViewServiceClient(ctx, endpoint.Address);

            var result = await Task.Factory.StartNew<bool>(proxy.Connect);
            if (result)
                this.connectedHosts.TryAdd(host, proxy);

            return result;
        }

        public bool Connect(SharingEndpoint endpoint)
        {
            return this.ConnectAsync(endpoint).Result;
        }

        public void Disconnect(SharingEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void RaiseOnScreenUpdateReceived(ScreenUpdateEventArgs e)
        {
            if (this.OnScreenUpdateReceived != null)
            {
                this.OnScreenUpdateReceived(this, e);
            }
        }

        private void ViewServiceOnOnImageChange(Image display, string id, string host)
        {
            this.RaiseOnScreenUpdateReceived(new ScreenUpdateEventArgs { Host = host, WindowId = id, Screen = display });
        }

        #endregion
    }
}