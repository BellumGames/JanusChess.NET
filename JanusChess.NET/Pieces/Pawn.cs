using System.Collections.Generic;
using System.Linq;

namespace JanusChess.NET
{
	public class Pawn : Piece
	{
		public Pawn(Player owner) : base(owner)
		{
			MovesNorth = Player.StartPosition == PlayerStartPosition.South;
		}

		public bool EnPassantVulnerable { get; } = false;

		/// <summary>
		/// Is true if this pawn moves and attacks towards the north side of the board, otherwise
		/// false for south.
		/// </summary>
		public bool MovesNorth { get; }

		public override string Name => nameof(Pawn);
		public override string Symbol => Color == PieceColor.Black ? "♟" : "♙"; // \u265F : \u2659

        public override int Value => 10;

        public override bool Attacks(Position pos)
		{
			int dx = pos.X - Position.X;
			int dy = pos.Y - Position.Y;

			if (MovesNorth && dx == -1 && (dy == -1 || dy == 1)) return true;
			if (!MovesNorth && dx == 1 && (dy == -1 || dy == 1)) return true;

			return false;
		}

		public override IEnumerable<Position> GetValidMoveDestinations()
		{
			int ds = MovesNorth ? 1 : -1; // direction sign
			return new Position?[]
			{
				Position.TryOffset(-1 * ds, -1), Position.TryOffset(-1 * ds, 0),
				Position.TryOffset(-1 * ds, 1), Position.TryOffset(-2 * ds, 0)
			}.Where(p => p.HasValue && ValidMoveTo(p.Value)).Select(p => p.Value);
		}

		public override bool ValidMoveTo(Position pos)
		{
			if (Position == pos) return false;
			Piece destinationPiece = Board[pos];

			// Destination must not contain a piece belonging to the same player.
			if (Player.Owns(destinationPiece)) return false;

			// If king is in check move is only valid if it defends king.
			if (Player.KingOnBoard.InCheck && !DefendsCheckIfMovedTo(pos)) return false;

			// It must not expose the king to an attack.
			if (ExposesKingToCheckIfMovedTo(pos)) return false;

			int dx = pos.X - Position.X;
			int dy = pos.Y - Position.Y;

			// Attack move on existing piece.
			if (destinationPiece != null) return Attacks(pos);
			// Forward move on free position.
			else
			{
				if (dy == 0 && dx == (MovesNorth ? -1 : 1)) return true;
				if (!Moved && dy == 0 && dx == (MovesNorth ? -2 : 2) &&
					Board[Position.Offset(dx / 2, dy)] == null)
					return true;
				return false;
			}

			// TODO: check for en passant move: https://en.wikipedia.org/wiki/En_passant
		}
	}
}
