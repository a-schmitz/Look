namespace look.communication.test.forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using look.common.Events;
    using look.communication.Model;

    public partial class Form1 : Form
    {

        private readonly Dictionary<string, SharingEndpoint> addresses = new Dictionary<string, SharingEndpoint>();
        RemoteSharer share;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RemoteContext.Instance.OnScreenUpdateReceived += RemoteContextOnOnScreenUpdateReceived;
            RemoteContext.Instance.OnHostConnected += InstanceOnOnHostConnected;
            RemoteContext.Instance.StartAcceptingConnections("Kekse");

            foreach (var endpoint in RemoteContext.Instance.FindClients())
            {
                var entry = string.Format("{0} ({1})", endpoint.Name, endpoint.Address.Uri.Host);
                addresses.Add(entry, endpoint);
                listBox1.Items.Add(entry);
            }
            
        }

        private void InstanceOnOnHostConnected(object sender, HostConnectedEventArgs e) {
            var result = MessageBox.Show(
                string.Format("Confirm? ({0})", e.Host),
                "Connection Request",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            e.Accepted = result == DialogResult.Yes;
        }

        private void RemoteContextOnOnScreenUpdateReceived(object sender, ScreenUpdateEventArgs e)
        {
            UpdateImage(e.Screen);
        }


        private SharingEndpoint endpoint;

        private void ThreadConnect()
        {
            var success = RemoteContext.Instance.Connect(endpoint);
            if (success) {
                share = new RemoteSharer(endpoint.Address.Uri.Host);
                share.Start();
            }
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            endpoint = addresses[((ListBox)sender).SelectedItem.ToString()];

            var connectScreen = new Thread(this.ThreadConnect);
            connectScreen.Start();

            //var success = RemoteContext.Instance.Connect(endpoint);
            //MessageBox.Show(success.ToString());
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
    }
}
