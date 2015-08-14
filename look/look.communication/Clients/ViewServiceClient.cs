namespace look.communication.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Discovery;

    using look.common.Model;
    using look.communication.Contracts;
    using look.communication.Hosts;
    using look.communication.Model;

    public class ViewServiceClient : DiscoveryBaseClient<IViewService>
    {
        public ViewServiceClient()
        {
        }

        public ViewServiceClient(EndpointAddress endpointAddress)
        {
            Endpoint.Address = endpointAddress;
        }

        public bool Connect()
        {
            return Channel.Connect();
        }

        public void Disconnect()
        {
            Channel.Disconnect();
        }

        public void PushWindowList(List<Window> windows)
        {
            Channel.PushAvailableWindows(windows);   
        }

        public void PushScreenUpdate(byte[] data)
        {
            Channel.PushScreenUpdate(data);
        }

        public string PushCursorUpdate(byte[] data)
        {
            return Channel.PushCursorUpdate(data);
        }

        public void RequestWindowTransfer(List<Window> windows)
        {
            Channel.RequestWindowTransfer(windows);
        }

        protected override SharingEndpoint TransformEndpoint(EndpointDiscoveryMetadata endpoint)
        {
            var scope = endpoint.Scopes.Select(s => s.ToString()).FirstOrDefault(s => s.StartsWith(ViewServiceHost.NAME_SCOPE));
            var name = scope != null ? scope.Remove(0, 15).TrimEnd('/') : "<unknown>";

            return new SharingEndpoint(name, endpoint.Address);
        }
    }
}
