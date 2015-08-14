namespace look.communication.Hosts
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;

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

        public bool Start<T>(string name) where T : IViewService
        {
            if (this.running)
                return true;

            var retryCount = 0;
            var retry = false;
            do
            {
                try
                {
                    // http://blogs.msdn.com/b/salvapatuel/archive/2007/04/25/why-using-is-bad-for-your-wcf-service-host.aspx
                    this.serviceHost = new ServiceHost(typeof(T), this.baseAddress);      
                    var behavior = new EndpointDiscoveryBehavior();
                    behavior.Scopes.Add(new Uri(string.Format("{0}{1}", NAME_SCOPE, name)));

                    var endpoint = this.serviceHost.Description.Endpoints.Find(typeof(IViewService));
                    if (endpoint != null)
                        endpoint.EndpointBehaviors.Add(behavior);

                    this.serviceHost.Open();

                }
                catch (AddressAlreadyInUseException)
                {
                    this.SetBaseAddress();

                    retry = true;
                    retryCount++;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            while (retry && retryCount < 3);

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
            // TODO port configurable
            var port = Convert.ToInt32(10000);
            this.baseAddress = new Uri(string.Format("net.tcp://{0}:{1}/viewservice/{2}", Dns.GetHostName(), port, Guid.NewGuid()));
        }

        #endregion

        public const string NAME_SCOPE = "net.tcp://name-";
    }
}
