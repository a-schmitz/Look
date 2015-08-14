namespace look.communication.Contracts
{
    using System.Collections.Generic;
    using System.ServiceModel;

    using look.communication.Model;

    [ServiceContract]
    public interface IViewService
    {
        #region bi-drectional

        [OperationContract(IsOneWay = false)]
        bool Connect();

        [OperationContract(IsOneWay = true)]
        void Disconnect();

        #endregion


        #region sender -> receiver

        [OperationContract(IsOneWay = true)]
        void PushAvailableWindows(List<Window> windows);

        [OperationContract(IsOneWay = true)]
        void PushScreenUpdate(byte[] data);

        [OperationContract(IsOneWay = false)]
        string PushCursorUpdate(byte[] data);

        #endregion
        

        #region receiver -> sender

        [OperationContract(IsOneWay = true)]
        void RequestWindowTransfer(List<Window> windows);

        #endregion
    }
}
