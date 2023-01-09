using System;
using System.Windows.Forms;

namespace JanusChess.NET
{
	internal class SquareClickEventArgs : EventArgs
	{
		public SquareClickEventArgs(Position position, MouseButtons button)
		{
			Position = position;
			X = position.X;
			Y = position.Y;
			Button = button;
		}

		public SquareClickEventArgs(int x, int y, MouseButtons button)
		{
			X = x;
			Y = y;
			Position = new Position(x, y);
			Button = button;
		}

		public MouseButtons Button { get; }
		public Position Position { get; }
		public int X { get; }
		public int Y { get; }
	}
}
