using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	public enum PieceColor
	{
		Black,
		White
	}

	/// <summary>
	/// Represents a chess piece.
	/// </summary>
	public abstract class Piece
	{
		/// <summary>
		/// Creates a new chess piece for the given owning player.
		/// </summary>
		/// <param name="player"></param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="player"/> is null.
		/// </exception>
		protected Piece(Player player)
		{
			Player = player ?? throw new ArgumentNullException(nameof(player));
			Color = player.PieceColor;
		}

		public GameBoard Board { get; private set; } = null;
		public PieceColor Color { get; }

		/// <summary>
		/// Returns false if the piece never moved and it's still in its initial position, otherwise false.
		/// </summary>
		public bool Moved { get; private set; } = false;

		/// <summary>
		/// The full name of the piece.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// The player that owns this piece.
		/// </summary>
		public Player Player { get; }

		/// <summary>
		/// If the piece is added to a game board, this is the position of this piece on the board.
		/// </summary>
		public Position Position { get; private set; } = default;

		/// <summary>
		/// The symbol character that represents this piece type on a drawn surface.
		/// </summary>
		public abstract string Symbol { get; }

		public abstract int Value { get; }

		/// <summary>
		/// Called by board when added. Sets Position and Board.
		/// </summary>
		/// <param name="board"></param>
		public void AddedToBoard(GameBoard board, Position position)
		{
			if (Board != null) throw new InvalidOperationException("Piece already added to a board.");
			if (board == null) throw new ArgumentNullException(nameof(board));
			if (board[position] != this) throw new InvalidOperationException("Invalid position. Piece not found.");

			Board = board;
			Position = position;
			Player.PiecesOnBoard.Add(this);
			if (this is King king) Player.KingOnBoard = king;
		}

		/// <summary>
		/// Returns true if the given position on the board is under attack by this piece.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public abstract bool Attacks(Position pos);

		/// <summary>
		/// Returns the destinations of all valid moves this piece can make on the board it is on.
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<Position> GetValidMoveDestinations();

		/// <summary>
		/// Called by board when piece is moved. Updates the position.
		/// </summary>
		/// <param name="destination"></param>
		public void MovedOnBoard(Position destination, bool simulation = false)
		{
			Position = destination;
            if (!Moved && !simulation) Moved = true;
		}

		/// <summary>
		/// Called by board when piece is being removed. Unsets Position and Board.
		/// </summary>
		/// <param name="board"></param>
		public void RemovedFromBoard(bool simulate = false)
		{
			Board = null;
			Position = default;
			if (!simulate)
			{
				Player.PiecesOnBoard.Remove(this);
			}
		}

		/// <summary>
		/// Returns true if this piece can move to the given destination position, otherwise false.
		/// This includes the valid possibility of attacking an enemy piece at said position.
		/// </summary>
		/// <param name="dest">The destination position to be checked.</param>
		/// <returns></returns>
		public virtual bool ValidMoveTo(Position dest)
		{
			// Basic implementation for checking if move is valid. The base rule is: if the piece
			// attacks the position and the position is not occupied by a friendly piece (it's either
			// empty or occupied by an enemy piece), then it's also valid to move there, as long as
			// the king will not be exposed to attack as a result of the move. This rule can be
			// overriden in special cases like the king and pawns, where the attacked positions do
			// not coincide with the positions the piece can move to.

			if (Position == dest) return false;

			// Destination must not contain a piece belonging to the same player.
			if (Player.Owns(Board[dest])) return false;

			// If attacks the destination, it can move there as well.
			if (!Attacks(dest)) return false;

			// If king is in check move is only valid if it defends king.
			if (Player.KingOnBoard.InCheck && !DefendsCheckIfMovedTo(dest)) return false;

			// It must not expose the king to an attack.
			if (ExposesKingToCheckIfMovedTo(dest)) return false;

			// Valid move.
			return true;
		}

		/// <summary>
		/// Returns true if the king is in check and moving this piece to <paramref name="pos"/>
		/// will eliminate the check.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		protected bool DefendsCheckIfMovedTo(Position pos)
		{
			King king = Player.KingOnBoard;

			// Nothing to defend if not in check.
			if (!king.InCheck) return false;

			// A single piece can't defend a double check.
			if (king.InDoubleCheck) return false;

			// Short if the move to pos takes the single attacking piece, making the move valid.
			if (king.CheckingPiece.Position == pos) return true;

			// Attacking piece must build a line with the king, a line which could be interrupted by
			// this piece to defend the king.
			if (!king.Position.LineTo(king.CheckingPiece.Position)) return false;
			if (!pos.LiesOnLine(king.Position, king.CheckingPiece.Position)) return false;
			return true;
		}

		/// <summary>
		/// Returns true if the king will be exposed to an attack by an opposing piece if this piece
		/// is moved to pos. Otherwise, the king will not be in check and this method returns false.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		protected bool ExposesKingToCheckIfMovedTo(Position pos)
		{
			King king = Player.KingOnBoard;
			Piece defender = this;

			// Can't expose king if piece was not defending, can't defend unless builds straight or
			// diagonal line with kings position.
			if (!Position.LineTo(king.Position)) return false;

			// We have a king K, this piece D that defends, and a possible attacker A. If D defends K
			// from A, all three are in a line. K is exposed only if D steps out of this line (it
			// can't expose K if it moves towards A or K).

			// Represents a step from K towards D
			int kdStepX = Math.Sign(defender.Position.X - king.Position.X);
			int kdStepY = Math.Sign(defender.Position.Y - king.Position.Y);

			// Represents a step from D towards the destination of D (new D)
			int dndStepX = Math.Sign(pos.X - defender.Position.X);
			int dndStepY = Math.Sign(pos.Y - defender.Position.Y);

			// If D remains on the line KDA, K isn't exposed. D can travel away and towards K.
			//if (!pos.LineTo(king.Position))
			//{
			//	return true;
			//}
			if ((kdStepX == dndStepX && kdStepY == dndStepY && pos.LineTo(king.Position)) || // D going away from K
				(kdStepX == (dndStepX * -1) && kdStepY == (dndStepY * -1) && pos.LineTo(king.Position))) // D going towards K
				return false;

			// D leaves the line K-D-A. Navigate to A (if any) and check if it can attack K.
			Position? p = king.Position.Offset(kdStepX, kdStepY);
			do
			{
				Piece a = Board[p.Value];
				if (a != null && a != defender && a.Player != Player)
				{
					// A found. Temporarily remove D from board and see if K is under attack.
					GameBoard board = Board;
					Position posD = defender.Position;
					//bool oldMoved = board[posD].Moved;
					board.RemovePiece(defender, simulate : true);
					//var pieceTaken = board[posD];		
					//board.MovePiece(new Move(defender, pos));

					bool exposed = a.Attacks(king.Position);

					board.AddPiece(defender, posD);
					//board.MovePiece(new Move(defender, posD));			
					//if (pieceTaken != null)
					//{
					//	board.AddPiece(pieceTaken, posD);
					//}
					//board[posD].Moved = oldMoved;

					return exposed;
				}
				p = p.Value.TryOffset(kdStepX, kdStepY);
			}
			while (p.HasValue);
			
			return false; // A not found, K is safe.
		}

		public override string ToString() => $"{Symbol} {Name}";
	}
}
