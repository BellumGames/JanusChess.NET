using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	public enum PlayerStartPosition
	{
		North,
		South
	}

	public class Player
	{
		public Player(string name, PieceColor pieceColor, PlayerStartPosition position)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Null or empty player name.");
			Name = name;
			PieceColor = pieceColor;
			StartPosition = position;
		}

		public string Name { get; }
		public PieceColor PieceColor { get; }
		public PlayerStartPosition StartPosition { get; }

		/// <summary>
		/// Returns true if this player owns the given piece (color match).
		/// </summary>
		/// <param name="piece"></param>
		/// <returns></returns>
		public bool Owns(Piece piece) => piece != null && PieceColor == piece.Color;

		/// <summary>
		/// Collection of pieces this player owns and are present on the board.
		/// </summary>
		public ISet<Piece> PiecesOnBoard { get; } = new HashSet<Piece>();

		public King KingOnBoard { get; set; }

        public IDecisionMaker DecisionMaker { get; set; }

        public override string ToString() => Name;
	}
}
