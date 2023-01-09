using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	public class Queen : Piece
	{
		public Queen(Player owner) : base(owner) { }

		public override string Name => nameof(Queen);
		public override string Symbol => Color == PieceColor.Black ? "♛" : "♕"; // \u265B : \u2655

        public override int Value => 90;

        public override bool Attacks(Position pos)
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");
			if (Position == pos) return false;

			// Check if destination is on straight line or diagonal.
			if (!Position.LineTo(pos)) return false;

			// Path to destination must be free of pieces.
			if (!Board.FreePathBetween(Position, pos)) return false;

			// Position under attack.
			return true;
		}

		public override IEnumerable<Position> GetValidMoveDestinations()
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");

			// For all directions:
			for (int dx = -1; dx <= 1; dx++)
			{
				for (int dy = -1; dy <= 1; dy++)
				{
					// Got direction (dx,dy). Go that way and collect valid positions.
					bool foundPiece = false;
					for (Position? p = Position.TryOffset(dx, dy); p.HasValue; p = p.Value.TryOffset(dx, dy))
					{
						if (!foundPiece && Board[p.Value] != null) foundPiece = true;

						if (ValidMoveTo(p.Value)) yield return p.Value;
						// Reached the end of this direction. Only break after finding piece. It's
						// possible to go through invalid positions and eventually reach a valid one
						// that defends the king in case of check.
						else if (foundPiece) break;
					}
				}
			}
		}
	}
}
