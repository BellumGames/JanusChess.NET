using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
	public partial class GameForm : Form
	{
		private readonly ChessBoardControl _boardControl;
		private readonly bool? _host;
		private Game _game = null;

		public GameForm(bool? host = null)
		{
			InitializeComponent();
			_boardControl = new ChessBoardControl
			{
				Dock = DockStyle.Fill
			};
			MainSplitContainer.Panel1.Controls.Add(_boardControl);

			Log = new Log();
			Log.OnNewEntry += (sender, args) =>
			{
				textBox1.AppendText(args.LogEntry.Message + Environment.NewLine);
			};
			_host = host;
		}

		internal Log Log { get; }

		public void CreateGame(GameNetworkConnection connection, bool host)
		{
			_game = new Game(connection, host, _boardControl, Log);
			Text = host ? "Host" : "Client";
		}

		protected void DisposeNonDesigner(bool disposing)
		{
			if (!disposing)
				return;

			_boardControl.Dispose();
		}

		private void GameForm_Shown(object sender, EventArgs e)
		{
			if (_host.HasValue)
				Location = new Point(Location.X + Size.Width / (_host.Value ? -2 : 2), Location.Y);

			var networkForm = new NetworkForm(this, _host);
			networkForm.ShowDialog();

			// Connection was not made.
			if (_game == null)
				Close();
		}

        internal void CreatePVPGame()
		{
            _game = new Game(_boardControl, Log);
		}

		internal void CreatePVAGame()
		{
			_game = new Game(_boardControl, Log, player2IsAI: true);
		}

		private async void GameForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			await (_game?.Disconnect() ?? Task.CompletedTask);
		}
	}
}