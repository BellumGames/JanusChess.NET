using System;
using System.Windows.Forms;

namespace JanusChess.NET
{
	internal partial class NetworkForm : Form
	{
		private readonly GameForm _gameForm;
		private readonly bool? _host;

		public NetworkForm(GameForm gameForm, bool? host = null)
		{
			InitializeComponent();
			_gameForm = gameForm;
			_host = host;
		}

		private void buttonHost_Click(object sender, EventArgs e)
		{
			new NetworkHostForm(_gameForm, _host).ShowDialog(_gameForm);
			Close();
		}

		private void buttonClient_Click(object sender, EventArgs e)
		{
			new NetworkClientForm(_gameForm, _host).ShowDialog(_gameForm);
			Close();
		}

		private void NetworkForm_Shown(object sender, EventArgs e)
		{
			if (_host == true)
				buttonHost_Click(sender, e);
			else if (_host == false)
				buttonClient_Click(sender, e);
		}

        private void btnNetwork_Click(object sender, EventArgs e)
        {
			btnPVP.Visible = btnPVE.Visible = btnNetwork.Visible = false;
			buttonClient.Location = btnPVE.Location;
			buttonHost.Location = btnPVP.Location;
			buttonClient.Visible = buttonHost.Visible = true;
        }

        private void btnPVP_Click(object sender, EventArgs e)
        {
			_gameForm.CreatePVPGame();
			Close();
		}

        private void btnPVE_Click(object sender, EventArgs e)
		{
			_gameForm.CreatePVAGame();
			Close();
		}
    }
}