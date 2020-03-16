using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace TicTacToe
{
    public partial class MultiplayerGame : Form
    {
        public TcpClient client = new TcpClient();
        public NetworkStream stream;
        public MultiplayerGame()
        {
            InitializeComponent();
        }

        public void SetLocalhost()
        {
            try
            {
                this.client = new TcpClient("127.0.0.1", ServerGUI.PORT);
                this.stream = this.client.GetStream();
            }
            catch (Exception)
            {
                SetLocalhost();
            }            
        }
        private void MultiplayerGame_Load(object sender, EventArgs e)
        {

        }

        private void MultiplayerGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(this.client == null)) this.client.Close();
            Application.Exit();
        }

        private void Connection_Click(object sender, EventArgs e)
        {
            MessageBox.Show(IsClientConnected().ToString());
        }

        private bool IsClientConnected()
        {
            return !((this.client.Client.Poll(1, SelectMode.SelectRead) && (this.client.Client.Available == 0)) || !this.client.Client.Connected);
        }
    }
}
