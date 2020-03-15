using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class ServerGUI : Form
    {
        public const int PORT = 5000;
        const string SERVER_IP = "0.0.0.0";

        private TcpClient client1;
        private NetworkStream stream1;

        TcpClient client2;
        private NetworkStream stream2;

        public ServerGUI()
        {
            InitializeComponent();
            ipBox.Text = new WebClient().DownloadString("http://icanhazip.com");
        }

        private void ServerGUI_Load(object sender, EventArgs e)
        {
        }

        private void ServerGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void ServerGUI_Shown(object sender, EventArgs e)
        {
            WaitForPlayers();
        }

        private void WaitForPlayers()
        {
            try
            {
                IPAddress address = IPAddress.Parse(SERVER_IP);
                TcpListener listener = new TcpListener(address, PORT);
                ConsoleTextBox.Text += "Waiting for players..." + Environment.NewLine;
                listener.Start();

                BackgroundWorker waitForFirstPlayer = new BackgroundWorker();

                waitForFirstPlayer.DoWork += (s, a) =>
                {
                    this.client1 = listener.AcceptTcpClient();
                    this.stream1 = client1.GetStream();
                };
                waitForFirstPlayer.RunWorkerCompleted += (s, a) =>
                {
                    ConsoleTextBox.Text += "Player 1 connected: " + client1.Client.RemoteEndPoint.ToString() + Environment.NewLine;
                    Players.Text += client1.Client.RemoteEndPoint.ToString() + Environment.NewLine;

                    BackgroundWorker waitForSecondPlayer = new BackgroundWorker();

                    waitForSecondPlayer.DoWork += (s1, a1) =>
                    {
                        this.client2 = listener.AcceptTcpClient();
                        this.stream2 = client2.GetStream();
                    };
                    waitForSecondPlayer.RunWorkerCompleted += (s1, a1) =>
                    {
                        ConsoleTextBox.Text += "player 2 connected: " + client2.Client.RemoteEndPoint.ToString() + Environment.NewLine;
                        Players.Text += client2.Client.RemoteEndPoint.ToString();
                        listener.Stop();
                    };
                    waitForSecondPlayer.RunWorkerAsync();
                };
                waitForFirstPlayer.RunWorkerAsync();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }
        }
    }
}
