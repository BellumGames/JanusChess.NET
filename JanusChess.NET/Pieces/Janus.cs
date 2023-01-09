using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanusChess.NET
{
  /// <summary>
  /// Bishop + knight
  /// </summary>
    public class Janus : Piece
    {
        public Janus(Player owner) : base(owner) { }

        public override string Name => nameof(Janus);

        public override string Symbol => Color == PieceColor.Black ? "🩓" : "🩐";

        public override int Value => 60;

        public override bool Attacks(Position pos)
        {
            if (Board == null) throw new InvalidOperationException("Piece not added to board.");
            if (Position == pos) return false;

            // Move must be L shaped jump.
            int dx = pos.X - Position.X;
            int dy = pos.Y - Position.Y;
            if ((Math.Abs(dx) == 1 && Math.Abs(dy) == 2) || (Math.Abs(dx) == 2 && Math.Abs(dy) == 1))
                return true; // Position under attack.


            // Check if destination is on straight diagonal.
            if (!Position.DiagonalLineTo(pos)) return false;

            // Path to destination must be free of pieces.
            if (!Board.FreePathBetween(Position, pos)) return false;

            // Position under attack.
            return true;
        }

        public override IEnumerable<Position> GetValidMoveDestinations()
        {
            if (Board == null) throw new InvalidOperationException("Piece not added to board.");

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

            // For all diagonal directions:
            for (int dx = -1; dx <= 1; dx += 2)
            {
                for (int dy = -1; dy <= 1; dy += 2)
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
