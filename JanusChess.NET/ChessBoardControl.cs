using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace JanusChess.NET
{
	/// <summary>
	/// This class is the UI part of the chess game. It exposes methods that can be used to display
	/// the visual representation of the board, layout of pieces, and possible piece moves.
	/// </summary>
	internal class ChessBoardControl : Control
	{
		private readonly PrivateFontCollection pfc = new();
		private const float FontEmSizeScaleFactor = 0.5F;

		// Ensures symbols are drawn centered inside the containing board squares.
		private static readonly StringFormat s_symbolFormat = new()
        {
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Far
			
		};

		// _boardDrawArea is divided in 8x8 rectangles represented by BoardSquare.
		private readonly BoardSquare[,] _boardSquares;

		// The calculated square area on this control that will represent the board.
		private RectangleF _boardDrawArea = default;

		// The calculated size of a square of the 8x8 board.
		//private float _boardSquareSize = default;
		private float _boardXSize = default;
		private float _boardYSize = default;

		public ChessBoardControl() : base()
		{
			// Force redraw the entire control on resize.
			ResizeRedraw = true;
			// Fixes redraw "flash".
			DoubleBuffered = true;
			pfc.AddFontFile("u1f800ttf.ttf");

			// Create the squares and set their background colors.
			_boardSquares = new BoardSquare[8, 10];
			for (int x = 0; x <= _boardSquares.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= _boardSquares.GetUpperBound(1); y++)
				{
					_boardSquares[x, y] = new BoardSquare((x + y) % 2 != 0); // Formula determines if dark square;
				}
			}
		}

		public delegate void SquareClickHandler(object sender, SquareClickEventArgs e);

		/// <summary>
		/// Event is fired when the user clicks on the squares of the board.
		/// </summary>
		public event SquareClickHandler OnSquareClick;

		/// <summary>
		/// Removes any symbols at the position p and redraws.
		/// </summary>
		/// <param name="p"></param>
		public void ClearSquare(Position p)
		{
			if (string.IsNullOrEmpty(_boardSquares[p.X, p.Y].Symbol)) return;
			_boardSquares[p.X, p.Y].Symbol = null;
			Invalidate(); // TODO: specific area
		}

		/// <summary>
		/// Draws the representing symbol for each chess piece in the game board layout at their
		/// proper positions on this control.
		/// </summary>
		/// <param name="layout"></param>
		public void Draw(GameBoard layout)
		{
			for (int x = 0; x <= layout.GetUpperBound(0); x++)
			{
				for (int y = 0; y <= layout.GetUpperBound(1); y++)
				{
					_boardSquares[x, y].Symbol = layout[x, y]?.Symbol;
				}
			}
			Invalidate();
		}

		/// <summary>
		/// Draws the symbol of the given piece at the given position p.
		/// </summary>
		/// <param name="piece"></param>
		/// <param name="p"></param>
		public void Draw(Piece piece, Position p)
		{
			_boardSquares[p.X, p.Y].Symbol = piece.Symbol;
			Invalidate(); // TODO: specific area
		}

		/// <summary>
		/// Sets the squares highlighted state to 'highlighted' for all the given positions.
		/// </summary>
		/// <param name="highlighted"></param>
		/// <param name="positions"></param>
		public void HighlightSquares(bool highlighted, IEnumerable<Position> positions)
		{
			foreach (Position p in positions) _boardSquares[p.X, p.Y].Highlighted = highlighted;
			Invalidate(); // TODO: specific area
		}

		/// <summary>
		/// Sets the square at position pos in a 'selected' selection state.
		/// </summary>
		/// <param name="selected"></param>
		/// <param name="pos"></param>
		public void SelectSquare(bool selected, Position pos)
		{
			_boardSquares[pos.X, pos.Y].Selected = selected;
			Invalidate(); // TODO: specific area
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			if (!_boardDrawArea.Contains(e.Location)) return;

			int x = (int)((e.Y - _boardDrawArea.Location.Y) / _boardYSize);
			int y = (int)((e.X - _boardDrawArea.Location.X) / _boardXSize);
			OnSquareClick?.Invoke(this, new SquareClickEventArgs(x, y, e.Button));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			//Debug.WriteLine("Paint " + e.ClipRectangle);

			Graphics g = e.Graphics;
			g.TextRenderingHint = TextRenderingHint.AntiAlias;

			//using (var darkBrush = new SolidBrush(_squareColorDark))
			//using (var lightBrush = new SolidBrush(_squareColorLight))
			//{
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					BoardSquare square = _boardSquares[x, y];
					// Draw the board square.
					//g.FillRectangle((x + y) % 2 == 0 ? lightBrush : darkBrush, _boardSquares[x, y]);
					using (var brush = new SolidBrush(square.Color)) g.FillRectangle(brush, square.Rectangle);

					// Draw the symbol of the piece, if any.
					if (!string.IsNullOrEmpty(square.Symbol))
						g.DrawString(square.Symbol, Font, Brushes.Black, square.Rectangle,
							s_symbolFormat);
				}
			}
			//}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			// Resize the drawing area for the board - biggest fitting square centered.
			if (Width > Height)
			{
				_boardDrawArea.X = (Width - Height) / 2;
				_boardDrawArea.Y = 0;
				_boardDrawArea.Width = Height;
				_boardDrawArea.Height = Height;
			}
			else
			{
				_boardDrawArea.X = 0;
				_boardDrawArea.Y = (Height - Width) / 2;
				_boardDrawArea.Width = Width;
				_boardDrawArea.Height = Width;
			}

			// Board square size.
			_boardXSize = _boardDrawArea.Height / 10;
			_boardYSize = _boardDrawArea.Width / 8;
			ResizeBoardSquares();
			// Update symbol font size.
			Font = new Font(pfc.Families[0], _boardYSize * FontEmSizeScaleFactor);

			// Redraw
			Invalidate();
		}

		/// <summary>
		/// Mixes color c1 and c2 using amountC2 to determine the percentage of c2 in the resulting
		/// color. The resulting color is c1 when amountC2 is 0 or lower, and c2 when amountC2 is 1
		/// or higher.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <param name="amountC2"></param>
		/// <returns></returns>
		private static Color ColorMix(Color c1, Color c2, double amountC2)
		{
			amountC2 = Math.Max(0, Math.Min(1, amountC2));
			int r = (byte)((c2.R * amountC2) + c1.R * (1 - amountC2));
			int g = (byte)((c2.G * amountC2) + c1.G * (1 - amountC2));
			int b = (byte)((c2.B * amountC2) + c1.B * (1 - amountC2));
			return Color.FromArgb(r, g, b);
		}

		// Recalculates and sets new values for square positions and sizes.
		private void ResizeBoardSquares()
		{
			float y = _boardDrawArea.Y;
			for (int i = 0; i < 8; i++)
			{
				float x = _boardDrawArea.X;
				for (int j = 0; j < 10; j++)
				{
					_boardSquares[i, j].Rectangle = new RectangleF(x, y, _boardYSize, _boardYSize);
					x += _boardXSize;
				}
				y += _boardYSize;
			}
		}

		/// <summary>
		/// Represents one square of the board. Saves values for background color, rectangle
		/// (position & size on the drawable area of the board), symbol of the piece at this
		/// position, and selection status.
		/// </summary>
		private class BoardSquare
		{
			private static readonly Color s_squareColorDark = Color.Silver;
			private static readonly Color s_squareColorHighlighted = Color.CornflowerBlue;
			private static readonly Color s_squareColorHighlightedSymbol = Color.OrangeRed;
			private static readonly Color s_squareColorLight = Color.Gainsboro;
			private static readonly Color s_squareColorSelected = Color.CornflowerBlue;
			private readonly Color _color;

			public BoardSquare(bool darkColored)
			{
				_color = darkColored ? s_squareColorDark : s_squareColorLight;
			}

			public Color Color
			{
				get
				{
					if (Selected) return s_squareColorSelected;
					else if (Highlighted && !string.IsNullOrEmpty(Symbol))
						return ColorMix(_color, s_squareColorHighlightedSymbol, 0.3);
					else if (Highlighted)
						return ColorMix(_color, s_squareColorHighlighted, 0.3);
					else return _color;
				}
			}

			public bool Highlighted { get; set; }

			/// <summary>
			/// The area on the drawable area of the board that this object represents.
			/// </summary>
			public RectangleF Rectangle { get; set; }

			public bool Selected { get; set; }

			/// <summary>
			/// The symbol of the piece at this position, if any.
			/// </summary>
			public string Symbol { get; set; }
		}
	}
}