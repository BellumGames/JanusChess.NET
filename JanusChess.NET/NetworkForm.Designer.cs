namespace JanusChess.NET
{
	partial class NetworkForm
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
            this.buttonHost = new System.Windows.Forms.Button();
            this.buttonClient = new System.Windows.Forms.Button();
            this.btnNetwork = new System.Windows.Forms.Button();
            this.btnPVE = new System.Windows.Forms.Button();
            this.btnPVP = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonHost
            // 
            this.buttonHost.Location = new System.Drawing.Point(43, 15);
            this.buttonHost.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonHost.Name = "buttonHost";
            this.buttonHost.Size = new System.Drawing.Size(308, 109);
            this.buttonHost.TabIndex = 1;
            this.buttonHost.Text = "Become Host";
            this.buttonHost.UseVisualStyleBackColor = true;
            this.buttonHost.Visible = false;
            this.buttonHost.Click += new System.EventHandler(this.buttonHost_Click);
            // 
            // buttonClient
            // 
            this.buttonClient.Location = new System.Drawing.Point(43, 134);
            this.buttonClient.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonClient.Name = "buttonClient";
            this.buttonClient.Size = new System.Drawing.Size(308, 109);
            this.buttonClient.TabIndex = 2;
            this.buttonClient.Text = "Connect as Client";
            this.buttonClient.UseVisualStyleBackColor = true;
            this.buttonClient.Visible = false;
            this.buttonClient.Click += new System.EventHandler(this.buttonClient_Click);
            // 
            // btnNetwork
            // 
            this.btnNetwork.Location = new System.Drawing.Point(82, 258);
            this.btnNetwork.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnNetwork.Name = "btnNetwork";
            this.btnNetwork.Size = new System.Drawing.Size(312, 109);
            this.btnNetwork.TabIndex = 5;
            this.btnNetwork.Text = "Player vs Network";
            this.btnNetwork.UseVisualStyleBackColor = true;
            this.btnNetwork.Click += new System.EventHandler(this.btnNetwork_Click);
            // 
            // btnPVE
            // 
            this.btnPVE.Location = new System.Drawing.Point(82, 137);
            this.btnPVE.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnPVE.Name = "btnPVE";
            this.btnPVE.Size = new System.Drawing.Size(312, 109);
            this.btnPVE.TabIndex = 4;
            this.btnPVE.Text = "Player vs AI";
            this.btnPVE.UseVisualStyleBackColor = true;
            this.btnPVE.Click += new System.EventHandler(this.btnPVE_Click);
            // 
            // btnPVP
            // 
            this.btnPVP.Location = new System.Drawing.Point(82, 15);
            this.btnPVP.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnPVP.Name = "btnPVP";
            this.btnPVP.Size = new System.Drawing.Size(312, 109);
            this.btnPVP.TabIndex = 3;
            this.btnPVP.Text = "Player vs Player";
            this.btnPVP.UseVisualStyleBackColor = true;
            this.btnPVP.Click += new System.EventHandler(this.btnPVP_Click);
            // 
            // NetworkForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 484);
            this.Controls.Add(this.btnNetwork);
            this.Controls.Add(this.btnPVE);
            this.Controls.Add(this.btnPVP);
            this.Controls.Add(this.buttonClient);
            this.Controls.Add(this.buttonHost);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NetworkForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NetworkForm";
            this.Shown += new System.EventHandler(this.NetworkForm_Shown);
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button buttonHost;
		private System.Windows.Forms.Button buttonClient;
        private System.Windows.Forms.Button btnNetwork;
        private System.Windows.Forms.Button btnPVE;
        private System.Windows.Forms.Button btnPVP;
    }
}