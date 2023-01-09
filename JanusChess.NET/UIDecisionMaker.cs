using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
    internal class UIDecisionMaker : IDecisionMaker
	{
		private readonly GameBoard _board;
        private readonly PieceColor _pieceColor;
        private readonly ChessBoardControl _boardControl;
        private UIState _state;
        private Piece _selectedPiece = null;
        private ICollection<Position> _selectedPieceValidMoves = null;

        public event DecisionTakenHandler DecisionTaken;

		public UIDecisionMaker(ChessBoardControl boardControl, GameBoard board, PieceColor pieceColor)
		{
            _boardControl = boardControl;
            _board = board;
            _pieceColor = pieceColor;
        }

        private void OnSquareClick(object sender, SquareClickEventArgs e)
        {
            //	// 1. selectare piesa
            //	// 2. click mutare piesa
            //	DecisionTaken?.Invoke(this, new SquareClickEventArgs(x, y));

            //if (_currentPlayerIndex != _localPlayerIndex)
            //	return;

            // Player selects piece to move using left button click.
            if (_state == UIState.PlayerSelectsOwnPiece && e.Button == MouseButtons.Left)
                HandlePieceSelectPVP(e.Position);

            // Player cancels piece selection with right click.
            else if (_state == UIState.PlayerMovesSelectedPiece && e.Button == MouseButtons.Right)
                HandlePieceUnselectPVP();

            // Player moves the selected piece to the clicked position (if move valid).
            else if (_state == UIState.PlayerMovesSelectedPiece && e.Button == MouseButtons.Left)
                HandlePieceMovePVP(e.Position);
        }

        private void HandlePieceSelectPVP(Position position)
        {
            if (_board[position]?.Color != _pieceColor)
                return; // Not your own piece, retry.

            _selectedPiece = _board[position];
            _boardControl.SelectSquare(true, position);
            _selectedPieceValidMoves = _selectedPiece.GetValidMoveDestinations().ToArray();
            _boardControl.HighlightSquares(true, _selectedPieceValidMoves);

            _state = UIState.PlayerMovesSelectedPiece;
        }

        private void HandlePieceUnselectPVP()
        {
            _boardControl.SelectSquare(false, _selectedPiece.Position);
            _boardControl.HighlightSquares(false, _selectedPieceValidMoves);

            _selectedPiece = null;
            _selectedPieceValidMoves = null;

            _state = UIState.PlayerSelectsOwnPiece;
        }

        private void HandlePieceMovePVP(Position position)
        {
            if (!_selectedPiece.ValidMoveTo(position))
                return; // Move not valid, retry.

            // Special case if castling - two pieces moved.
            //if (!_selectedPiece.Moved && _selectedPiece is King king &&
            //    king.ValidatedCastlingMove != null)
            //{
                // Castling. Move the rook as well.
                //MakeMovePVP(king.ValidatedCastlingMove);
                //DecisionTaken?.Invoke(this, new DecisionTakenEventArgs { Move = king.ValidatedCastlingMove});
            //}

            // Make the move.
            var move = new Move(_selectedPiece, position);

            // Update visuals
            _boardControl.SelectSquare(false, move.Origin);
            _boardControl.HighlightSquares(false, _selectedPieceValidMoves);
            _selectedPiece = null;
            _selectedPieceValidMoves = null;


            _state = UIState.PlayerSelectsOwnPiece;
            _boardControl.OnSquareClick -= OnSquareClick;

            DecisionTaken?.Invoke(this, new DecisionTakenEventArgs { Move = move });
        }

        public void StartThinkingAboutNextMove()
        {
            _state = UIState.PlayerSelectsOwnPiece;
            _boardControl.OnSquareClick += OnSquareClick;
        }

        public enum UIState
        {
            PlayerSelectsOwnPiece,
            PlayerMovesSelectedPiece
        }
    }
}
