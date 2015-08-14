namespace look.communication.Hosts
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;

    using look.common.Helper;
    using look.communication.Contracts;

    public class ViewServiceHost
    {
        #region fields

        private Uri baseAddress;

        private bool running;

        private ServiceHost serviceHost;

        #endregion

        #region constructors

        public ViewServiceHost()
        {
            this.running = false;
            this.SetBaseAddress();
        }

        #endregion

        #region public functions

        public bool Start<T>(string name) where T : IViewService {
            if (this.running)
                return true;

            try {
                // http://blogs.msdn.com/b/salvapatuel/archive/2007/04/25/why-using-is-bad-for-your-wcf-service-host.aspx
                this.serviceHost = new ServiceHost(typeof(T), this.baseAddress);
                var behavior = new EndpointDiscoveryBehavior();
                behavior.Scopes.Add(new Uri(string.Format("{0}{1}", NAME_SCOPE, name)));

                var endpoint = this.serviceHost.Description.Endpoints.Find(typeof(IViewService));
                if (endpoint != null)
                    endpoint.EndpointBehaviors.Add(behavior);

                this.serviceHost.Open();

            } catch (Exception) {
                return false;
            }

            this.running = true;

            return true;
        }

        public void Stop()
        {
            if (!this.running)
                return;

            this.serviceHost.Close();
            ((IDisposable)this.serviceHost).Dispose();

            this.running = false;
        }

        #endregion

        #region private functions

        private void SetBaseAddress()
        {
            this.baseAddress = new Uri(string.Format(BASE_ADDRESS, IpHelper.GetLocalIp(), PORT));
        }

        #endregion

        public const string NAME_SCOPE = "net.tcp://name-";
        public const string BASE_ADDRESS = "net.tcp://{0}:{1}/viewservice/";
        // TODO port configurable
        public const int PORT = 10000;

    }
}
