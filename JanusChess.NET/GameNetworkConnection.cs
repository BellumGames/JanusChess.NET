using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JanusChess.NET
{
	public class GameNetworkConnection
	{
		private readonly TcpClient _tcpClient;
		private readonly ISynchronizeInvoke _invocationTarget;
		private readonly NetworkStream _stream;

		public delegate Task NetworkMessageHandler(GameNetworkMessage message);

		public event NetworkMessageHandler ReceivedMessage;

		public GameNetworkConnection(TcpClient tcpClient, ISynchronizeInvoke invocationTarget)
		{
			_tcpClient = tcpClient;
			_invocationTarget = invocationTarget;
			_stream = tcpClient.GetStream();

			Task.Run(ReadLoopAsync);
		}

		public async Task SendAsync(GameNetworkMessage message)
		{
			string messageString = JsonConvert.SerializeObject(message);
			byte[] data = Encoding.UTF8.GetBytes(messageString);
			byte[] datalen = BitConverter.GetBytes(data.Length);

			Debug.WriteLine($"Sending message ({data.Length}): {messageString}");

			await _stream.WriteAsync(datalen, offset: 0, count: datalen.Length);
			await _stream.WriteAsync(data, offset: 0, count: data.Length);
			await _stream.FlushAsync();
		}

		private async Task ReadLoopAsync()
		{
			byte[] datalenbuffer = new byte[sizeof(int)];
			byte[] databuffer = new byte[256];

			while (_tcpClient.Connected)
			{
				await _stream.ReadAsync(datalenbuffer, offset: 0, sizeof(int));
				int datalen = BitConverter.ToInt32(datalenbuffer, startIndex: 0);

				if (datalen > databuffer.Length)
					databuffer = new byte[datalen];

				await _stream.ReadAsync(databuffer, offset: 0, count: datalen);
				GameNetworkMessage message = JsonConvert.DeserializeObject<GameNetworkMessage>(
					Encoding.UTF8.GetString(databuffer, index: 0, count: datalen));

				if (!_invocationTarget.InvokeRequired)
				{
					await ReceivedMessage.Invoke(message);
					return;
				}

				_invocationTarget.Invoke(ReceivedMessage, new object[] { message });
			}
		}
	}
}