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
        public TcpClient client;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connecting to server");
            }            
        }
        private void MultiplayerGame_Load(object sender, EventArgs e)
        {

        }

        private void MultiplayerGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
