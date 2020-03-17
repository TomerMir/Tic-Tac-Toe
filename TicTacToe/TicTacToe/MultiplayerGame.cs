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
using System.Threading;

namespace TicTacToe
{
    public partial class MultiplayerGame : Form
    {
        public TcpClient client = new TcpClient();
        public NetworkStream stream;
        private Board board;
        private bool isO;
        public MultiplayerGame()
        {
            InitializeComponent();
            this.board = new Board(this);
            this.Controls.Add(this.board);
            
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
            Thread reciveWhoStarts = new Thread(() =>
            {
                this.isO = ReciveFromServer() == 11 ? true : false;
                this.connection.Invoke((MethodInvoker)delegate ()
                {
                    connection.Text = isO.ToString();
                });
                if (!this.isO)
                {
                    this.board.DisableAll();
                    int index = ReciveFromServer();
                    this.board.EnableAll();
                    if (index == 202)
                    {
                        MessageBox.Show("Opponent disconnected, you won!");
                        Application.Exit();
                    }
                    else
                    {
                        ChancgeCellValue(board.GetCellByIndex(index), 2);
                    }
                }
            });
            reciveWhoStarts.Start();
        }

        private int ReciveFromServer()
        {
            byte[] buffer = new byte[1];
            int length = this.stream.Read(buffer, 0, 1);
            return buffer[0];
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

        public void Button_Clicked(object sender, EventArgs e)
        {
            Cell clickedCell = sender as Cell;
            if (clickedCell.GetValue() != 0) return;
            this.board.DisableAll();
            clickedCell.SetTextByValue(isO ? 2 : 1);
            int whoWins = Board.IsThrereAWinner(board.GetValues());
            if (whoWins == 2)
            {
                MessageBox.Show("You Won!");
            }
            else if (whoWins == 1)
            {
                MessageBox.Show("Tie :(");
            }
            byte[] bufferIndex = new byte[1] { (byte)clickedCell.GetIndex() };
            this.stream.Write(bufferIndex, 0, 1);

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, a) =>
            {
                this.stream.Read(bufferIndex, 0, 1);
            };
            worker.RunWorkerCompleted += (s, a) =>
            {
                board.GetCellByIndex(bufferIndex[0]).SetTextByValue(isO ? 1 : 2);
                whoWins = Board.IsThrereAWinner(board.GetValues());
                if (whoWins == 2)
                {
                    MessageBox.Show("You Lost :(");
                }
                else if (whoWins == 1)
                {
                    MessageBox.Show("Tie :(");
                }
                this.board.EnableAll();
            };
            worker.RunWorkerAsync();
        }

        private void ChancgeCellValue(Cell cell, int value)
        {
            cell.Invoke((MethodInvoker)delegate ()
            {
                cell.SetTextByValue(value);
            });
        }

        private bool IsClientConnected()
        {
            return !((this.client.Client.Poll(1, SelectMode.SelectRead) && (this.client.Client.Available == 0)) || !this.client.Client.Connected);
        }
    }
}
