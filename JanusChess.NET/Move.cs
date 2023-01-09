using Newtonsoft.Json;
using System;

namespace JanusChess.NET
{
	public class Move
	{
		/// <summary>
		/// Creates a new move object. Piece must not be null and be added to a board.
		/// </summary>
		/// <param name="piece"></param>
		/// <param name="destination"></param>
		public Move(Piece piece, Position destination)
		{
			Piece = piece ?? throw new ArgumentNullException(nameof(piece));
			if (Piece.Board == null) throw new InvalidOperationException("Piece not on a board.");
			Destination = destination;
			Origin = piece.Position;
		}

		[JsonConstructor]
		private Move(Position origin, Position destination)
		{
			Origin = origin;
			Destination = destination;
		}

		public Position Destination { get; }
		public Position Origin { get; }

		[JsonIgnore]
		public Piece Piece { get; }

		public override string ToString()
		{
			return $"{Piece.Symbol}|{Piece.Name} to {Destination}";
		}
	}
}
