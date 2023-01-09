namespace JanusChess.NET
{
	partial class NetworkHostForm
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
			this.textBoxPort = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonHost = new System.Windows.Forms.Button();
			this.labelStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBoxPort
			// 
			this.textBoxPort.Location = new System.Drawing.Point(86, 10);
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(154, 31);
			this.textBoxPort.TabIndex = 0;
			this.textBoxPort.Text = "45678";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 25);
			this.label1.TabIndex = 1;
			this.label1.Text = "Port:";
			// 
			// buttonHost
			// 
			this.buttonHost.Location = new System.Drawing.Point(18, 62);
			this.buttonHost.Name = "buttonHost";
			this.buttonHost.Size = new System.Drawing.Size(464, 57);
			this.buttonHost.TabIndex = 2;
			this.buttonHost.Text = "Host";
			this.buttonHost.UseVisualStyleBackColor = true;
			this.buttonHost.Click += new System.EventHandler(this.buttonHost_ClickAsync);
			// 
			// labelStatus
			// 
			this.labelStatus.AutoSize = true;
			this.labelStatus.Location = new System.Drawing.Point(13, 126);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(0, 25);
			this.labelStatus.TabIndex = 3;
			// 
			// NetworkHostForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(494, 324);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.buttonHost);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxPort);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NetworkHostForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "NetworkHostForm";
			this.Shown += new System.EventHandler(this.NetworkHostForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonHost;
		private System.Windows.Forms.Label labelStatus;
	}
}