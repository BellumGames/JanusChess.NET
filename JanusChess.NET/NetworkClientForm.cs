using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
	internal partial class NetworkClientForm : Form
	{
		private readonly GameForm _gameForm;
		private readonly bool? _host;

		public NetworkClientForm(GameForm gameForm, bool? host)
		{
			InitializeComponent();
			_gameForm = gameForm;
			_host = host;
		}

		private async void buttonConnect_Click(object sender, EventArgs e)
		{
			var tcpClient = new TcpClient();
			await tcpClient.ConnectAsync(textBoxIP.Text, int.Parse(textBoxPort.Text));
			_gameForm.CreateGame(new GameNetworkConnection(tcpClient, _gameForm), host: false);

			Close();
		}

		private async void NetworkClientForm_Shown(object sender, EventArgs e)
		{
			if (_host == false)
			{
				await Task.Delay(1000); // give a chance for the host to start listening.
				buttonConnect_Click(sender, e);
			}
		}
	}
}
