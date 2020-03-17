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

        private TcpClient clientO;
        private NetworkStream streamO;

        TcpClient clientX;
        private NetworkStream streamX;

        private Random rnd = new Random();

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
            if (!(this.clientO == null)) this.clientO.Close();
            if (!(this.clientX == null)) this.clientX.Close();
            Application.Exit();
        }

        private void ServerGUI_Shown(object sender, EventArgs e)
        {
            WaitForPlayers();

            Thread gameThread = new Thread(() =>
            {
                while (true)
                {
                    if (IsFinishedConnecting()) break;
                }
                AddText(ConsoleTextBox, "Starting game...");
                DecideWhoStarts();
                SendWhoStarts();
                RunGame();
            });
            gameThread.Start();
        }

        private void DecideWhoStarts()
        {
            int random = this.rnd.Next(1, 3);
            if (random == 2)
            {
                TcpClient tmpClient = this.clientO;
                NetworkStream tmpStream = this.streamO;
                this.clientO = this.clientX;
                this.clientX = tmpClient;
                this.streamO = this.streamX;
                this.streamX = tmpStream;
            }
        }

        private void SendWhoStarts()
        {
            byte[] buffer = new byte[1] { 11 };
            this.streamO.Write(buffer, 0, 1);
            buffer[0] = 10;
            this.streamX.Write(buffer, 0, 1);
        }

        private void RunGame()
        {
            while (true)
            {
                try
                {
                    byte[] bufferIndex = new byte[1];

                    if (!IsClientConnected(clientO))
                    {
                        AddText(ConsoleTextBox, "Player O disconnected");
                        return;
                    }
                    streamO.Read(bufferIndex, 0, 1);
                    AddText(ConsoleTextBox, "player O has played at index " + bufferIndex[0].ToString());

                    if (!IsClientConnected(clientX))
                    {
                        AddText(ConsoleTextBox, "Player X disconnected");
                        return;
                    }
                    streamX.Write(bufferIndex, 0, 1);
                    AddText(ConsoleTextBox, "sending to player X index " + bufferIndex[0].ToString());

                    if (!IsClientConnected(clientX))
                    {
                        AddText(ConsoleTextBox, "Player X disconnected");
                        return;
                    }
                    streamX.Read(bufferIndex, 0, 1);
                    AddText(ConsoleTextBox, "player X has played at index " + bufferIndex[0].ToString());

                    if (!IsClientConnected(clientO))
                    {
                        AddText(ConsoleTextBox, "Player O disconnected");
                        return;
                    }
                    streamO.Write(bufferIndex, 0, 1);
                    AddText(ConsoleTextBox, "sending to player O index " + bufferIndex[0].ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                
            }
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
                    this.clientO = listener.AcceptTcpClient();
                    this.streamO = clientO.GetStream();
                };
                waitForFirstPlayer.RunWorkerCompleted += (s, a) =>
                {
                    ConsoleTextBox.Text += "Player 1 connected: " + clientO.Client.RemoteEndPoint.ToString() + Environment.NewLine;
                    Players.Text += clientO.Client.RemoteEndPoint.ToString() + Environment.NewLine;

                    BackgroundWorker waitForSecondPlayer = new BackgroundWorker();

                    waitForSecondPlayer.DoWork += (s1, a1) =>
                    {
                        this.clientX = listener.AcceptTcpClient();
                        this.streamX = clientX.GetStream();
                    };
                    waitForSecondPlayer.RunWorkerCompleted += (s1, a1) =>
                    {
                        ConsoleTextBox.Text += "player 2 connected: " + clientX.Client.RemoteEndPoint.ToString() + Environment.NewLine;
                        Players.Text += clientX.Client.RemoteEndPoint.ToString();
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

        private void AddText(TextBox box, string text)
        {
            box.Invoke((MethodInvoker)delegate ()
            {
                box.Text += text + Environment.NewLine;
            });
        }

        public bool IsFinishedConnecting()
        {
            return (this.clientO != null && this.clientX != null && this.streamO != null && this.streamX != null);
        }

        private bool IsClientConnected(TcpClient client)
        {
            return !((client.Client.Poll(1, SelectMode.SelectRead) && (client.Client.Available == 0)) || !client.Client.Connected);
        }
    }
}
