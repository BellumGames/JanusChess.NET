using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
    /// <summary>
    /// Class that represents a game of chess. It handles the flow and all events of the game.
    /// </summary>
    internal class Game
    {
        private readonly GameBoard _board;
        private readonly GameNetworkConnection _connection;
        private readonly bool _host;
        private readonly ChessBoardControl _boardControl;
        private int _currentPlayerIndex = -1;
        private readonly Log _log;
        private readonly Player[] _players;
        private readonly int _localPlayerIndex;
        private GameState _state;
        private bool _disconnected = false;

        /// <summary>
        /// Creates a new game of chess. The given <paramref name="control"/> will be used to draw
        /// the visual representation of this game.
        /// </summary>
        /// <param name="control"></param>
        public Game(GameNetworkConnection connection, bool host, ChessBoardControl control, Log log)
        {
            _connection = connection;
            _host = host;
            _boardControl = control ?? throw new ArgumentNullException(nameof(control));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _board = new GameBoard();

            var uiDecisionMaker = new UIDecisionMaker(
                _boardControl,
                _board,
                host ? PieceColor.White : PieceColor.Black);
            var networkDecisionMaker = new NetworkDecisionMaker(_connection, _board);

            _players = new Player[]
            {
                new Player(
                    $"Player 1{(host ? " (you)" : string.Empty)}",
                    PieceColor.White,
                    PlayerStartPosition.South),
                new Player(
                    $"Player 2{(!host ? " (you)" : string.Empty)}",
                    PieceColor.Black,
                    PlayerStartPosition.North)
            };
            _players[0].DecisionMaker = host ? uiDecisionMaker : networkDecisionMaker;
            _players[1].DecisionMaker = host ? networkDecisionMaker : uiDecisionMaker;

            _board.AddAllPieces(NewStandardPieceLayout(_players[0], _players[1]));
            _boardControl.Draw(_board);

            _connection.ReceivedMessage += OnReceivedMessageAsync;


            if (_host)
            {
                // Start the game.
                control.Invoke((Action)HostGameStartAsync);
                //CurrentPlayer = _players[0];
            }
            //CurrentPlayer = _players[1];

            _localPlayerIndex = host ? 0 : 1;
        }

        public Game(ChessBoardControl boardControl, Log log, bool player2IsAI = false)
        {
            _boardControl = boardControl ?? throw new ArgumentNullException(nameof(boardControl));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _board = new GameBoard();
            _players = new Player[]
            {
                new Player("Player 1", PieceColor.White, PlayerStartPosition.South),
                new Player(
                    "Player 2",
                    PieceColor.Black,
                    PlayerStartPosition.North)
            };

            _players[0].DecisionMaker = new UIDecisionMaker(boardControl, _board, PieceColor.White);
            //_players[0].DecisionMaker = player2IsAI
            //    ? new AIDecisionMaker(_players[0], _board, _boardControl, _players[1])
            //    : new UIDecisionMaker(boardControl, _board, PieceColor.Black);
            _players[1].DecisionMaker = player2IsAI
                ? new AIDecisionMaker(_players[1], _board, _boardControl, _players[0])
                : new UIDecisionMaker(boardControl, _board, PieceColor.Black);

            _board.AddAllPieces(NewStandardPieceLayout(_players[0], _players[1]));
            _boardControl.Draw(_board);

            _localPlayerIndex = 0;
            _state = GameState.Playing;
            SetNextPlayerAsync(send: false, playerId: 0).Wait();
            //CurrentPlayer = _players[_currentPlayerIndex];
        }

        private async void HostGameStartAsync()
        {
            await SetNextPlayerAsync(); // Start with player[0]
        }

        private Player CurrentPlayer => _players[_currentPlayerIndex];

        /// <summary>
        /// Creates a new standard chess 8x8 piece layout (2d array) for the two given players.
        /// </summary>
        /// <param name="southPlayer"></param>
        /// <param name="northPlayer"></param>
        /// <returns></returns>
        private static Piece[,] NewStandardPieceLayout(Player southPlayer, Player northPlayer)
        {
            return new Piece[8, 10]
            {
                { new Rook(northPlayer), new Janus(northPlayer), new Knight(northPlayer), new Bishop(northPlayer), new Queen(northPlayer),
                    new King(northPlayer), new Bishop(northPlayer), new Knight(northPlayer),new Janus(northPlayer), new Rook(northPlayer)},
                { new Pawn(northPlayer), new Pawn(northPlayer), new Pawn(northPlayer), new Pawn(northPlayer),new Pawn(northPlayer),
                    new Pawn(northPlayer), new Pawn(northPlayer), new Pawn(northPlayer), new Pawn(northPlayer), new Pawn(northPlayer) },
                { null, null, null, null, null, null, null,  null, null, null },
                { null, null, null, null, null, null, null,  null, null, null },
                { null, null, null, null, null, null, null,  null, null, null },
                { null, null, null, null, null, null, null,  null, null, null },
                { new Pawn(southPlayer), new Pawn(southPlayer), new Pawn(southPlayer), new Pawn(southPlayer), new Pawn(southPlayer),
                    new Pawn(southPlayer), new Pawn(southPlayer), new Pawn(southPlayer), new Pawn(southPlayer), new Pawn(southPlayer) },
                { new Rook(southPlayer),new Janus(southPlayer), new Knight(southPlayer), new Bishop(southPlayer), new Queen(southPlayer),
                    new King(southPlayer), new Bishop(southPlayer), new Knight(southPlayer), new Janus(southPlayer), new Rook(southPlayer)},
            };
        }

        /// <summary>
        /// Sets the current player to the next.
        /// </summary>
        private async Task SetNextPlayerAsync(bool send = true, int? playerId = null)
        {
            _currentPlayerIndex = playerId ?? ++_currentPlayerIndex % _players.Length;
            _log.Write($"It is {CurrentPlayer}'s turn.");

            if (send)
                await (_connection?.SendAsync(new GameNetworkMessage
                {
                    Type = MessageType.SetCurrentPlayer,
                    Player = _currentPlayerIndex
                }) ?? Task.CompletedTask);

            _players[_currentPlayerIndex].DecisionMaker.DecisionTaken += HandleDecisionTakenAsync;
            _players[_currentPlayerIndex].DecisionMaker.StartThinkingAboutNextMove();
        }

        private async Task UpdateCheckStatus(Move move, bool send = true)
        {
            // Update check status of opposing king.
            Player opponent = _players[(_currentPlayerIndex + 1) % 2];
            opponent.KingOnBoard.UpdateCheckStatus(move);
            if (opponent.KingOnBoard.InCheck)
            {
                if (PlayerInCheckMate(opponent))
                {
                    _log.Write($"{opponent} is in check mate. {CurrentPlayer} wins the game!");
                    await SetStateAsync(GameState.GameOver, send);
                    return;
                }
                else _log.Write($"{opponent} is in check!");
            }
        }

        private async void HandleDecisionTakenAsync(object sender, DecisionTakenEventArgs e)
        {
            _players[_currentPlayerIndex].DecisionMaker.DecisionTaken -= HandleDecisionTakenAsync;
            if (_host || _connection == null)
            {
                if (e.Move != null)
                    await MakeMoveAsync(e.Move);
            }
            else
                await (_connection?.SendAsync(new GameNetworkMessage
                {
                    Type = MessageType.DecisionTaken,
                    Move = e.Move
                }) ?? Task.CompletedTask);
        }

        private async Task MakeMoveAsync(Move move, bool send = true)
        {
            Piece pieceTaken = _board[move.Destination];
            _log.Write($"{CurrentPlayer} moves {move.Piece} from {move.Origin} to {move.Destination}" +
                (pieceTaken == null ? "." : $" and takes {pieceTaken}."));

            _board.MovePiece(move);
            _boardControl.ClearSquare(move.Origin); // clear old square
            _boardControl.Draw(move.Piece, move.Piece.Position); // draw piece on new square

            if (send)
            {
                await (_connection?.SendAsync(new GameNetworkMessage
                {
                    Type = MessageType.MakeMove,
                    Move = move
                }) ?? Task.CompletedTask);
            }
            
            await UpdateCheckStatus(move, send);
            if (_state != GameState.GameOver)
            {
                await SetNextPlayerAsync(false);
            }
        }

        private async Task SetStateAsync(GameState state, bool send = true)
        {
            _state = state;

            if (send)
                await (_connection?.SendAsync(new GameNetworkMessage
                {
                    Type = MessageType.SetState,
                    State = state
                }) ?? Task.CompletedTask);
        }

        private async Task OnReceivedMessageAsync(GameNetworkMessage message)
        {
            switch (message.Type)
            {
                case MessageType.Disconnected:
                    _disconnected = true;
                    MessageBox.Show("Disconnected.");
                    _log.Write("Disconnected. Game over.");
                    break;
                case MessageType.SetState:
                    await SetStateAsync(message.State.Value, send: false);
                    break;
                case MessageType.SetCurrentPlayer:
                    await SetNextPlayerAsync(send: false, playerId: message.Player.Value);
                    break;
                case MessageType.MakeMove:
                    await MakeMoveAsync(new Move(_board[message.Move.Origin], message.Move.Destination), send: false);
                    break;
            }
        }

        private static bool PlayerInCheckMate(Player player)
        {
            // Must be in check first.
            if (!player.KingOnBoard.InCheck) return false;

            // Check all pieces for valid moves that eliminate the check.
            foreach (Piece p in player.PiecesOnBoard)
                if (p.GetValidMoveDestinations().Any())
                    return false;

            return true;
        }

        public async Task Disconnect()
        {
            if (!_disconnected)
                await (_connection?.SendAsync(new GameNetworkMessage
                {
                    Type = MessageType.Disconnected
                }) ?? Task.CompletedTask);
        }
    }
}
