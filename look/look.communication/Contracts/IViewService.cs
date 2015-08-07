namespace look.communication.Contracts
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IViewService
    {
        [OperationContract]
        void PushScreenUpdate(byte[] data);

        [OperationContract]
        string PushCursorUpdate(byte[] data);

        [OperationContract]
        string Ping();
    }
}
