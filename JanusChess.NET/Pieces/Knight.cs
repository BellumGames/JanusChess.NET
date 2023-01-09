using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	public class Knight : Piece
	{
		public Knight(Player owner) : base(owner) { }

		public override string Name => nameof(Knight);
		public override string Symbol => Color == PieceColor.Black ? "♞" : "♘"; // \u265E : \u2658

        public override int Value => 30;

        public override bool Attacks(Position pos)
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");
			if (Position == pos) return false;

			// Move must be L shaped jump.
			int dx = pos.X - Position.X;
			int dy = pos.Y - Position.Y;
			if ((Math.Abs(dx) == 1 && Math.Abs(dy) == 2) || (Math.Abs(dx) == 2 && Math.Abs(dy) == 1))
				return true; // Position under attack.

			// Not valid.
			return false;
		}

		public override IEnumerable<Position> GetValidMoveDestinations()
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");

			// For all possible jumps:
			int[] dxs = { -2, -2, -1, -1, 1, 1, 2, 2 };
			int[] dys = { -1, 1, -2, 2, -2, 2, -1, 1 };

			for (int i = 0; i < dxs.Length; i++)
			{
				int dx = dxs[i];
				int dy = dys[i];

				// Got offset (dx,dy). Check if valid position.
				Position? p = Position.TryOffset(dx, dy);
				if (p.HasValue && ValidMoveTo(p.Value)) yield return p.Value;
			}
		}
	}
}
