using System;
using System.Collections.Generic;

namespace JanusChess.NET
{
	public class GameBoard
	{
		private readonly Piece[,] _pieces;

		/// <summary>
		/// Creates a game board with no pieces.
		/// </summary>
		public GameBoard()
		{
			_pieces = new Piece[8, 10];
		}

        /// <summary>
        /// Creates a game board and adds all the pieces to it. <paramref name="pieces"/> must be a
        /// 2d array of exactly 8x8 size.
        /// </summary>
        /// <param name="pieces"></param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="pieces"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="pieces"/> is not two dimentional.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="pieces"/> is not 8x8 in size.
        /// </exception>
		public GameBoard(Piece[,] pieces) : this()
		{
            if (pieces == null) throw new ArgumentNullException(nameof(pieces));
			if (pieces.Rank != 2)
				throw new ArgumentException($"{pieces} must be a two dimentional array.", nameof(pieces));
			if (pieces.GetUpperBound(0) != 7 || pieces.GetUpperBound(1) != 9)
				throw new ArgumentOutOfRangeException($"{pieces} must be an 8x8 array.");

			AddAllPieces(pieces);
		}

		public Piece this[int x, int y] => _pieces[x, y];

		public Piece this[Position p] => _pieces[p.X, p.Y];

		/// <summary>
		/// Adds all the pieces from this 2d array to the board at the same positions. The array must
		/// be smaller than the board (8x10).
		/// </summary>
		/// <param name="pieces"></param>
		public void AddAllPieces(Piece[,] pieces)
		{
			for (int x = 0; x <= pieces.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= pieces.GetUpperBound(1); y++)
				{
					if (pieces[x, y] != null) AddPiece(pieces[x, y], new Position(x, y));
				}
			}
		}

        /// <summary>
        /// Adds <paramref name="piece"/> at <paramref name="position"/> on this board. Also handles
        /// updating the given piece.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="position"></param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="piece"/> is null, already added on a board, or <paramref
        /// name="position"/> is already occupied.
        /// </exception>
		public void AddPiece(Piece piece, Position position)
		{
			if (piece == null || piece.Board != null || this[position] != null)
				throw new InvalidOperationException("Can't add piece to board.");

			_pieces[position.X, position.Y] = piece;
			piece.AddedToBoard(this, position);
		}

		/// <summary>
		/// Returns true if the path between p1 and p2 is a valid straight or diagonal path and is
		/// free of pieces (start and end position excluded), i.e. a piece can be moved from one of
		/// the positions to the other.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public bool FreePathBetween(Position p1, Position p2)
		{
			// Path must be a valid straight or diagonal line.
			if (!p1.LineTo(p2)) return false;

			// Determine a single step from p1 towards p2.
			int dx = Math.Sign(p2.X - p1.X);
			int dy = Math.Sign(p2.Y - p1.Y);

			// Path to p2 must be free of pieces.
			p1 = p1.Offset(dx, dy);
			for (; p1 != p2; p1 = p1.Offset(dx, dy))
				if (this[p1] != null)
					return false; // Piece in the way.

			// Path is free and valid.
			return true;
		}

        /// <summary>
        /// Gets the index of the last element of the specified dimension int the underlying array of pieces.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
		public int GetUpperBound(int dimension) => _pieces.GetUpperBound(dimension);

        /// <summary>
        /// Moves the piece from it's current position to the move destination, and removes the piece
        /// at the destination position if there is one.
        /// </summary>
        /// <param name="move"></param>
		public void MovePiece(Move move, bool simulation = false)
		{
            if (move == null) throw new ArgumentNullException(nameof(move));

			Piece pieceAtDestination = _pieces[move.Destination.X, move.Destination.Y];
			if (pieceAtDestination != null) RemovePieceAt(move.Destination);
			_pieces[move.Destination.X, move.Destination.Y] = move.Piece;
			_pieces[move.Piece.Position.X, move.Piece.Position.Y] = null;
			move.Piece.MovedOnBoard(move.Destination, simulation);
		}

        /// <summary>
        /// Returns true if there is at least one piece of opposing color to <paramref
        /// name="defendingPieceColor"/> on the board that attacks the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="defendingPieceColor"></param>
        /// <returns></returns>
		public bool PositionUnderAttack(Position position, PieceColor defendingPieceColor)
		{
			foreach (Piece a in _pieces)
			{
				if (a != null && a.Color != defendingPieceColor && a.Attacks(position)) return true;
			}
			return false;
		}

        /// <summary>
        /// Removes the piece <paramref name="p"/> from this board. The piece must already be present
        /// on the board.
        /// </summary>
        /// <param name="p"></param>
		public void RemovePiece(Piece p, bool simulate = false)
		{
            if (p == null) throw new ArgumentNullException(nameof(p));
            if (p.Board != this || p != _pieces[p.Position.X, p.Position.Y]) throw new InvalidOperationException();
			RemovePieceAt(p.Position, simulate);
		}

        /// <summary>
        /// Removes the piece present at the given <paramref name="position"/> from the board, if any.
        /// </summary>
        /// <param name="position"></param>
		public void RemovePieceAt(Position position, bool simulate = false)
		{
			Piece p = _pieces[position.X, position.Y];
			if (p == null) return;
			_pieces[position.X, position.Y] = null;
			p.RemovedFromBoard( simulate);
		}
	}
}
