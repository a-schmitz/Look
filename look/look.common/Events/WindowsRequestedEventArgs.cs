namespace look.common.Events
{
    using System.Collections.Generic;

    using look.common.Model;

    public class WindowsRequestedEventArgs
    {
        public string Ip { get; set; }

        public List<Window> Windows { get; set; }
    }
}