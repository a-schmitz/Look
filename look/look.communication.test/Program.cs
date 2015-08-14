namespace look.communication.test
{
    using System;
    using System.Drawing;

    using look.communication.Clients;
    using look.communication.Hosts;
    using look.communication.Services;

    class Program
    {
        static void Main(string[] args)
        {
            ViewServiceHost.Instance.Start<ViewService>("Ich");
            ViewService.OnImageChange += ViewServiceOnOnImageChange;

            using (var client = new ViewServiceClient())
            {
                foreach (var endpoint in client.Discover())
                {
                    Console.WriteLine("Found {0} ({1})", endpoint.Item1, endpoint.Item2);
                    
                }
            }

            Console.Read();
        }

        private static void ViewServiceOnOnImageChange(Image display, string remoteIpAddress)
        {
            Console.WriteLine("Image changed: " + remoteIpAddress);
        }

        private static void Share()
        {
            //_threadScreen = new Thread(ScreenThread);
            //_threadScreen.Start();

            //_threadCursor = new Thread(CursorThread);
            //_threadCursor.Start();
        }
    }
}
