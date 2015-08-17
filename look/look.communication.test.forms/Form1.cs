namespace look.communication.test.forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using look.capture;
    using look.common.Events;
    using look.common.Model;

    public partial class Form1 : Form
    {

        private readonly Dictionary<string, string> addresses = new Dictionary<string, string>();
        private readonly List<Tuple<string, string, IntPtr>> windows = new List<Tuple<string, string, IntPtr>>();
        RemoteSharer share;

        private string ip;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RemoteContext.Instance.OnHostConnected += InstanceOnOnHostConnected;
            RemoteContext.Instance.OnWindowsShared += InstanceOnOnWindowsShared;
            RemoteContext.Instance.OnWindowsRequested += InstanceOnOnWindowsRequested;
            RemoteContext.Instance.OnScreenUpdateReceived += RemoteContextOnOnScreenUpdateReceived;
            RemoteContext.Instance.OnHostDisconnected += InstanceOnOnHostDisconnected;
            RemoteContext.Instance.StartAcceptingConnections("Kekse");

            foreach (var host in RemoteContext.Instance.FindClients()) {
                var entry = string.Format("{0} ({1})", host.Name, host.Ip);
                addresses.Add(entry, host.Ip);
                listBox1.Items.Add(entry);
            }

            windows.Add(new Tuple<string, string, IntPtr>(Guid.NewGuid().ToString(), "Desktop", IntPtr.Zero));

            foreach (var p in Process.GetProcesses().Where(process => !string.IsNullOrEmpty(process.MainWindowTitle))) {
                windows.Add(new Tuple<string, string, IntPtr>(Guid.NewGuid().ToString(), p.MainWindowTitle, p.MainWindowHandle));
                listBox2.Items.Add(p.MainWindowTitle);
            }
        }

        private void InstanceOnOnHostDisconnected(object sender, HostDisconnectedEventArgs e) {
            share.Stop();
            MessageBox.Show(e.Ip + " disconnected", "Disconnect");
        }

        private void InstanceOnOnWindowsRequested(object sender, WindowsRequestedEventArgs e) {
            share = new RemoteSharer(e.Ip);
            var id = e.Windows.First().Id;
            var handle = windows.First(entry => entry.Item1 == id).Item3;
            share.Start(handle);
        }

        private void InstanceOnOnWindowsShared(object sender, WindowsSharedEventArgs e) {
            var result = MessageBox.Show(
                string.Format("{0} shared: {1}", e.Ip, string.Join(", ", e.Windows.Select(w => w.Name))),
                "Accept Sharing?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes) {
                RemoteContext.Instance.RequestWindowTransfer(e.Ip, e.Windows);
            }
        }

        private void InstanceOnOnHostConnected(object sender, HostConnectedEventArgs e) {
            var result = MessageBox.Show(
                string.Format("Confirm? ({0})", e.Ip),
                "Connection Request",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            e.Accepted = result == DialogResult.Yes;
        }

        private void RemoteContextOnOnScreenUpdateReceived(object sender, ScreenUpdateEventArgs e)
        {
            UpdateImage(e.Screen);
        }


        private void ThreadConnect()
        {
            var success = RemoteContext.Instance.Connect(ip);
            if (!success)
                return;

            string title;
            try {
                title = GetSelectedItem();
            } catch (NullReferenceException) {
                title = "Desktop";
            }

            var id = windows.First(entry => entry.Item2 == title).Item1;

            var w = new Window { Id = id, Name = title };
            RemoteContext.Instance.ShareWindows(ip, new List<Window> { w });
        }

        private delegate string GetSelectedItemDelegate();
        private string GetSelectedItem() {
            if (listBox2.InvokeRequired)
                return Invoke(new GetSelectedItemDelegate(this.GetSelectedItem)).ToString();
            return listBox2.SelectedItem.ToString();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            ip = addresses[((ListBox)sender).SelectedItem.ToString()];

            var connectScreen = new Thread(this.ThreadConnect);
            connectScreen.Start();
        }

        private delegate void UpdateImageDelegate(Image img);
        private void UpdateImage(Image img)
        {
            if (pictureBox1.InvokeRequired)
            {
                Invoke(new UpdateImageDelegate(UpdateImage), new object[] { img });
            }
            else
            {
                pictureBox1.BackgroundImage = img;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
            share.Stop();
            share.Dispose();
            RemoteContext.Instance.StopAcceptingConnections();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RemoteContext.Instance.Disconnect(ip);
        }
    }
}
