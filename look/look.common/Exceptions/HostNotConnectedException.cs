namespace look.common.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class HostNotConnectedException : Exception
    {
        public string Host { get; set; }

        public HostNotConnectedException() {}

        public HostNotConnectedException(string message)
            : base(message) {}

        public HostNotConnectedException(string message, Exception innerException)
            : base(message, innerException) {}

        protected HostNotConnectedException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            if (info != null) {
                this.Host = info.GetString("Host");
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);

            if (info != null) {
                info.AddValue("Host", this.Host);
            }
        }
    }
}
