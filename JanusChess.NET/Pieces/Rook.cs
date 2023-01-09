using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	public class Rook : Piece
	{
		public Rook(Player owner) : base(owner) { }

		public override string Name => nameof(Rook);
		public override string Symbol => Color == PieceColor.Black ? "♜" : "♖"; // \u265C : \u2656

        public override int Value => 50;

        public override bool Attacks(Position pos)
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");
			if (Position == pos) return false;

			// Check if destination is on straight line
			if (!Position.StraightLineTo(pos)) return false;

			// Path to destination must be free of pieces.
			if (!Board.FreePathBetween(Position, pos)) return false;

			// Position under attack.
			return true;
		}

		public override IEnumerable<Position> GetValidMoveDestinations()
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");

			// For all straight directions:
			int[] dxs = { -1, 0, 1, 0 };
			int[] dys = { 0, 1, 0, -1 };

			for (int i = 0; i < dxs.Length; i++)
			{
				int dx = dxs[i];
				int dy = dys[i];

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
