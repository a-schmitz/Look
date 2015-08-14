namespace look.communication.Model
{
    using System.ServiceModel;

    public class SharingEndpoint
    {
        public string Name { get; set; }
        public EndpointAddress Address { get; set; }

        public SharingEndpoint(string name, EndpointAddress address)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}
