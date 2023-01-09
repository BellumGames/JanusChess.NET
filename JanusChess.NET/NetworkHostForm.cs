using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
	internal partial class NetworkHostForm : Form
	{
		private readonly GameForm _gameForm;
		private readonly bool? _host;
		private TcpListener _tcpListener;

		public NetworkHostForm(GameForm gameForm, bool? host = null)
		{
			InitializeComponent();
			_gameForm = gameForm;
			_host = host;
		}

		private async void buttonHost_ClickAsync(object sender, EventArgs e)
		{
			int port = int.Parse(textBoxPort.Text);
			textBoxPort.Enabled = false;
			buttonHost.Enabled = false;

			_tcpListener = new TcpListener(IPAddress.Any, port);
			_tcpListener.Start();

			string[] addresses = Dns
				.GetHostEntry(Dns.GetHostName())
				.AddressList
				.Where(addr => addr.AddressFamily == AddressFamily.InterNetwork)
				.Select(addr => addr.ToString())
				.ToArray();

			labelStatus.Text = $"Listening for new connections on port {port}." + Environment.NewLine
				+ "Detected network IPs:" + Environment.NewLine
				+ string.Join(Environment.NewLine, addresses);

			var tcpClient = await _tcpListener.AcceptTcpClientAsync();
			await Task.Delay(500);
			_gameForm.CreateGame(new GameNetworkConnection(tcpClient, _gameForm), host: true);
			Close();
		}

		private void NetworkHostForm_Shown(object sender, EventArgs e)
		{
			if (_host == true)
				buttonHost_ClickAsync(sender, e);
		}
	}
}