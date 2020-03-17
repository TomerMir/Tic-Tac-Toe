namespace TicTacToe
{
    partial class MultiplayerGame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connection
            // 
            this.connection.Location = new System.Drawing.Point(352, 1);
            this.connection.Name = "connection";
            this.connection.Size = new System.Drawing.Size(92, 22);
            this.connection.TabIndex = 0;
            this.connection.Text = "Test Connectoin";
            this.connection.UseVisualStyleBackColor = true;
            this.connection.Click += new System.EventHandler(this.Connection_Click);
            // 
            // MultiplayerGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 549);
            this.Controls.Add(this.connection);
            this.Name = "MultiplayerGame";
            this.Text = "Tic Tac Toe";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultiplayerGame_FormClosing);
            this.Load += new System.EventHandler(this.MultiplayerGame_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button connection;
    }
}