using System;
using System.Diagnostics;

namespace JanusChess.NET
{
	/// <summary>
	/// Represents a valid position in the layout, on the game board. [0..7]
	/// </summary>
	[DebuggerDisplay("X = {X}, Y = {Y}")]
	public struct Position
	{
		public Position(int x, int y)
		{
			if (x < 0 || x >= 8) throw new ArgumentOutOfRangeException(nameof(x));
			if (y < 0 || y >= 10) throw new ArgumentOutOfRangeException(nameof(y));

			X = x;
			Y = y;
		}

		public int X { get; private set; }
		public int Y { get; private set; }

		/// <summary>
		/// Returns true if the two positions build a diagonal (45dg) line on the board.
		/// </summary>
		public bool DiagonalLineTo(Position dest)
		{
			if (this == dest) return false;

			int dx = dest.X - X;
			int dy = dest.Y - Y;

			// -1,-1        -1,1
			//         0,0
			//  1,-1         1,1

			return Math.Abs(dx) == Math.Abs(dy);
		}

		/// <summary>
		/// Returns true if this position and position <paramref name="p"/> build either a straight
		/// or diagonal line on the board.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool LineTo(Position p) => DiagonalLineTo(p) || StraightLineTo(p);

		/// <summary>
		/// Determines if this position lies on the straight or diagonal line from <paramref
		/// name="a"/> to <paramref name="b"/>. If a and b don't build a line or if this position is
		/// not situated on that line, false is returned. All three positions must be unique.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public bool LiesOnLine(Position a, Position b)
		{
			if (a == b || a == this || b == this) return false;
			if (!a.LineTo(b) || !a.LineTo(this)) return false;

			int acdx = X - a.X;
			int acdy = Y - a.Y;
			int abdx = b.X - a.X;
			int abdy = b.Y - a.Y;

			// AC must be the same direction as AB
			if (Math.Sign(acdx) != Math.Sign(abdx) || Math.Sign(acdy) != Math.Sign(abdy)) return false;

			// C must be between A and B, not further than B.
			if (Math.Abs(acdx) > Math.Abs(abdx) || Math.Abs(acdy) > Math.Abs(abdy)) return false;

			return true;
		}

		/// <summary>
		/// Returns true if the two positions build either a vertical or horizontal straight line on
		/// the board.
		/// </summary>
		public bool StraightLineTo(Position dest)
		{
			if (this == dest) return false;

			int dx = dest.X - X;
			int dy = dest.Y - Y;

			//        -1,0
			//  0,-1   0,0   0,1
			//         1,0

			return dx == 0 || dy == 0;
		}

		public static bool operator ==(Position p1, Position p2) => p1.X == p2.X && p1.Y == p2.Y;

		public static bool operator !=(Position left, Position right) => !(left == right);

		public override int GetHashCode() => unchecked(X ^ Y);

		public override bool Equals(object obj)
		{
			if (!(obj is Position)) return false;
			var comp = (Position)obj;
			return comp.X == X && comp.Y == Y;
		}

		/// <summary>
		/// Returns this position translated by the specified amount.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the resulting position is out of the bounds of the board.
		/// </exception>
		public Position Offset(int x, int y)
		{
			Position? p = TryOffset(x, y);
			return p ?? throw new ArgumentOutOfRangeException();
		}

		/// <summary>
		/// Returns this position translated by the specified amount if it remains on the board,
		/// otherwise null.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Position? TryOffset(int x, int y)
		{
			int newx = X + x;
			int newy = Y + y;
			if (newx < 0 || newx >= 8) return null;
			if (newy < 0 || newy >= 10) return null;
			return new Position(newx, newy);
		}

		/// <summary>
		/// Returns true if this position is exactly one square away from the given position.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public bool AdjacentTo(Position p)
		{
			return this != p && Math.Abs(X - p.X) <= 1 && Math.Abs(Y - p.Y) <= 1;
		}

		/// <summary>
		/// Returns the chess notation of this position.
		/// </summary>
		public override string ToString() => $"{(char)(65 + Y)}{8 - X}";
	}
}
