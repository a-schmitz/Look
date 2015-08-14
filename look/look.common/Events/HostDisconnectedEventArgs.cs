namespace look.common.Events
{
    using System;

    public class HostDisconnectedEventArgs : EventArgs
    {
        public string Ip { get; set; }
    }
}
