namespace look.common.Events
{
    using System;

    public class HostConnectedEventArgs : EventArgs
    {
        public string Host { get; set; }

        public bool Accepted { get; set; }
    }
}
