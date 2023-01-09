using Newtonsoft.Json;

namespace JanusChess.NET
{
	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class GameNetworkMessage
	{
		public MessageType Type { get; set; }
		public int? Player { get; set; }
		public GameState? State { get; set; }
		public Move Move { get; set; }
	}
}