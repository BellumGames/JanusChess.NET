using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
    class AIDecisionMaker : IDecisionMaker
    {
        private readonly Player _player;
        private readonly GameBoard _board;
        private readonly ChessBoardControl _boardControl;
        private readonly Player _opponent;
        private Task thinkingTask;
        private static Random s_random = new Random();

        public AIDecisionMaker(Player player, GameBoard board, ChessBoardControl boardControl, Player opponent)
        {
            _player = player;
            _board = board;
            _boardControl = boardControl;
            _opponent = opponent;
        }

        public event DecisionTakenHandler DecisionTaken;
        public static int ctor = 0;
        public void StartThinkingAboutNextMove()
        {
            thinkingTask = Task.Run(Think);
        }

        private void Think()
        {
            //Thread.Sleep(1000);

            var (_, bestMoveEvah) = Minimax(_player, 5, -10000, 10000);

            _boardControl.Invoke((MethodInvoker)delegate { DecisionTaken?.Invoke(this, new DecisionTakenEventArgs { Move = bestMoveEvah }); });
        }

        private (int value, Move move) Minimax(Player player, int depth, int alpha, int beta)
        {
            var maximize = player == _player;

            if (depth == 0)
                return (EvaluateState(player), null);
            var validMoves = player.PiecesOnBoard.SelectMany(p => p.GetValidMoveDestinations().Select(d => new Move(p, d))).ToList();

            if (maximize)
            {
                var max = int.MinValue;
                List<Move> bestMoves = new List<Move>();
                var aux = max;
                foreach (var move in validMoves)
                {
                    var origin = move.Origin;
                    var pieceTaken = _board[move.Destination];
                    _board.MovePiece(move, simulation: true);
                    var (value, _) = Minimax(maximize ? _opponent : _player, depth - 1, alpha, beta);
                    alpha = Math.Max(alpha, value);

                    _board.MovePiece(new Move(move.Piece, origin), simulation: true);
                    if (pieceTaken != null)
                    {
                        _board.AddPiece(pieceTaken, move.Destination);
                    }
                    if (value > max)
                    {
                        max = value;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }
                    else if (value == max) 
                        bestMoves.Add(move); 

                    if (beta <= alpha)
                        break;
                }
                //if (aux != max)
                //{
                //    return (max, validMoves[s_random.Next(validMoves.Count - 1)]);
                //}
                return (max, bestMoves.Count == 0 ? null : bestMoves[s_random.Next(bestMoves.Count -1)]);
            }
            else
            {
                var min = int.MaxValue;
                List<Move> bestMoves = new List<Move>();
                var aux = min;
                foreach (var move in validMoves)
                {
                    var origin = move.Origin;
                    var pieceTaken = _board[move.Destination];
                    _board.MovePiece(move, simulation: true);

                    var (value, _) = Minimax(maximize ? _opponent : _player, depth - 1, alpha, beta);
                    beta = Math.Min(beta, value);

                    _board.MovePiece(new Move(move.Piece, origin), simulation: true);
                    if (pieceTaken != null)
                    {
                        _board.AddPiece(pieceTaken, move.Destination);
                    }

                    if (value < min)
                    {
                        min = value;
                        bestMoves.Clear();
                        bestMoves.Add(move);
                    }
                    else if (value == min) 
                        bestMoves.Add(move);
                    if (beta <= alpha)
                        break;
                }
                //if (aux != min)
                //{
                //    return (min, validMoves[s_random.Next(validMoves.Count - 1)]);
                //}
                return (min, bestMoves.Count == 0 ? null : bestMoves[s_random.Next(bestMoves.Count - 1)]);
            }
        }

        private int EvaluateState(Player player)
        {
            int sum = _player.PiecesOnBoard.Sum(p => p.Value) - _opponent.PiecesOnBoard.Sum(p => p.Value);
            return player.PieceColor.Equals(PieceColor.White) ? sum : -sum;
        }
    }
}
