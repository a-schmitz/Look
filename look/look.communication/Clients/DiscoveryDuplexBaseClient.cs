namespace look.communication.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;
    using System.Threading.Tasks;

    using look.communication.Model;

    public abstract class DiscoveryDuplexBaseClient<T> : DuplexClientBase<T> where T : class
    {
        #region constructor

        protected DiscoveryDuplexBaseClient() : base(new InstanceContext(new object()))
        {
        }

        protected DiscoveryDuplexBaseClient(InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        #endregion

        #region public functions

        public IEnumerable<SharingEndpoint> Discover(IEnumerable<string> scopes = null)
        {
            var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

            var services = discoveryClient.Find(this.CreateFindCriteria(scopes));

            discoveryClient.Close();

            if (services.Endpoints.Count < 1)
                yield break;

            foreach (var endpointDiscoveryMetadata in services.Endpoints)
            {
                yield return this.TransformEndpoint(endpointDiscoveryMetadata);
            }
        }

        public async Task<IEnumerable<SharingEndpoint>> DiscoverAsync(IEnumerable<string> scopes = null)
        {
            var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

            var services = await discoveryClient.FindTaskAsync(this.CreateFindCriteria(scopes));

            discoveryClient.Close();

            return services.Endpoints.Select(this.TransformEndpoint);
        }

        #endregion

        #region private functions

        private FindCriteria CreateFindCriteria(IEnumerable<string> scopes)
        {
            var findCriteria = new FindCriteria(typeof(T))
            {
                Duration = new TimeSpan(0, 0, 3)
            };

            if (scopes == null)
                return findCriteria;

            foreach (var scope in scopes)
                findCriteria.Scopes.Add(new Uri(scope));

            return findCriteria;
        }

        protected virtual SharingEndpoint TransformEndpoint(EndpointDiscoveryMetadata endpoint)
        {
            return new SharingEndpoint(endpoint.Version.ToString(), endpoint.Address);
        }


        #endregion
    }
}
