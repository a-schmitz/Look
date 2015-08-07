namespace look.communication.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;
    using System.Threading.Tasks;

    public abstract class DiscoveryBaseClient<T> : ClientBase<T> where T : class
    {
        #region public functions

        public IEnumerable<EndpointAddress> Discover(IEnumerable<string> scopes = null)
        {
            var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

            var services = discoveryClient.Find(this.CreateFindCriteria(scopes));

            discoveryClient.Close();

            if (services.Endpoints.Count < 1)
                yield break;

            foreach (var endpointDiscoveryMetadata in services.Endpoints)
                yield return endpointDiscoveryMetadata.Address;
        }

        public async Task<IEnumerable<EndpointAddress>> DiscoverAsync(IEnumerable<string> scopes = null)
        {
            var discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

            var services = await discoveryClient.FindTaskAsync(this.CreateFindCriteria(scopes));

            discoveryClient.Close();

            return services.Endpoints.Select(endpoint => endpoint.Address);
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

        #endregion
    }
}
