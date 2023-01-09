using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	public class King : Piece
	{
		public King(Player owner) : base(owner) { }

		public Piece CheckingPiece { get; private set; }
		public Piece CheckingPiece2 { get; private set; }
		public bool InCheck => CheckingPiece != null;
		public bool InDoubleCheck => InCheck && CheckingPiece2 != null;
		public override string Name => nameof(King);
		public override string Symbol => Color == PieceColor.Black ? "♚" : "♔"; // \u265A : \u2654

		/// <summary>
		/// If the last validated move was a castle move, this value saves the move that the rook
		/// needs to do in order to finish the castle. Otherwise this value is null.
		/// </summary>
		public Move ValidatedCastlingMove { get; private set; } = null;

        public override int Value => 900;

        public override bool Attacks(Position pos)
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");
			return Position.AdjacentTo(pos);
		}

		public override IEnumerable<Position> GetValidMoveDestinations()
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");

			// For all surrounding positions:
			for (int dx = -1; dx <= 1; dx++)
			{
				for (int dy = -1; dy <= 1; dy++)
				{
					// Got offset (dx,dy). Check if valid position.
					Position? p = Position.TryOffset(dx, dy);
					if (p.HasValue && ValidMoveTo(p.Value)) yield return p.Value;
				}
			}
			// Check castle positions as well:
			if (!Moved)
			{
				Position? cp = Position.TryOffset(0, -2);
				if (cp.HasValue && ValidMoveTo(cp.Value)) yield return cp.Value;
				cp = Position.TryOffset(0, 2);
				if (cp.HasValue && ValidMoveTo(cp.Value)) yield return cp.Value;
			}
		}

		/// <summary>
		/// Verifies if this king is in check as a result of the last move the opponent made and
		/// updates the describing properties accordingly. Should be called after each move.
		/// </summary>
		/// <param name="lastOpponentMove"></param>
		public void UpdateCheckStatus(Move lastOpponentMove)
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");
			CheckingPiece = null;
			CheckingPiece2 = null;

			// This king K can be in check as a result of the moving opponent piece's P arrival at
			// the destination position, but also as a result of P leaving its original position
			// possibly leaving free attack path for a ranged piece R.

			if (lastOpponentMove.Piece.Attacks(Position)) CheckingPiece = lastOpponentMove.Piece;

			// Navigate to R, check if exists and attacks this postion.
			if (!Position.StraightLineTo(lastOpponentMove.Origin)) return;
			int dx = lastOpponentMove.Origin.X - Position.X;
			int dy = lastOpponentMove.Origin.Y - Position.Y;
			Position? p = lastOpponentMove.Origin.TryOffset(dx, dy);
			for (; p.HasValue; p = p.Value.TryOffset(dx, dy))
			{
				Piece r = Board[p.Value];
				if (r != null)
				{
					// Found R
					if (r.Player != Player && r.Attacks(Position))
					{
						// R exists and attacks K.
						if (CheckingPiece == null) CheckingPiece = r;
						else CheckingPiece2 = r;
					}
					break;
				}
			}
		}

		public override bool ValidMoveTo(Position pos)
		{
			if (Board == null) throw new InvalidOperationException("Piece not added to board.");

			// Reset the castling state if needed.
			if (ValidatedCastlingMove != null) ValidatedCastlingMove = null;

			if (Position == pos) return false;
			Piece destinationPiece = Board[pos];

			// Destination must not contain a piece belonging to the same player.
			if (Player.Owns(destinationPiece)) return false;

			// Check for castle move if this is the first move.
			if (!Moved && ValidCastleMove(pos)) return true;

			// Destination must be adjacent to current position.
			if (!Position.AdjacentTo(pos)) return false;

			// Destination must not be under attack.
			if (Board.PositionUnderAttack(pos, Color)) return false;

			// Valid move.
			return true;
		}

		private bool ValidCastleMove(Position pos)
		{
			// The king can castle (rocadă) under certain circumstances: Both the king and the rook
			// must have never been moved during this game. Squares between king and rook must be
			// free. King must not be in check and must not cross over or end up in a position in
			// which it would be in check. The move is made by the king moving two squares towards
			// the rook.

			if (Moved)
				return false;
			int dy = pos.Y - Position.Y;
			if (Math.Abs(dy) != 2)
				return false;

			if (Board[Position.X, (dy == -2 ? 0 : 9)] is not Rook rook || rook.Moved || rook.Player != Player)
				return false;
			if (!Board.FreePathBetween(Position, rook.Position))
				return false;

			var castlePosition = new Position(Position.X, Position.Y + (dy / 2)); // where the rook lands.
			if (Board.PositionUnderAttack(Position, Color) ||
				Board.PositionUnderAttack(castlePosition, Color) ||
				Board.PositionUnderAttack(pos, Color))
				return false;

			// Save the move the rook has to do.
			ValidatedCastlingMove = new Move(rook, castlePosition);
			return true;
		}
	}
}
