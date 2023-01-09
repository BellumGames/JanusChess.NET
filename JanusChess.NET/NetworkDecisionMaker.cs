using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JanusChess.NET
{
    class NetworkDecisionMaker : IDecisionMaker
    {
        private readonly GameNetworkConnection _connection;
        private readonly GameBoard _board;

        public NetworkDecisionMaker(GameNetworkConnection connection, GameBoard board)
        {
            _connection = connection;
            _board = board;
        }

        public event DecisionTakenHandler DecisionTaken;

        public void StartThinkingAboutNextMove()
        {
            _connection.ReceivedMessage += OnReceivedMessageAsync;

        }

        private Task OnReceivedMessageAsync(GameNetworkMessage message)
        {
            switch (message.Type)
            {
                case MessageType.DecisionTaken:
                    _connection.ReceivedMessage -= OnReceivedMessageAsync;

                    DecisionTaken?.Invoke(this, new DecisionTakenEventArgs
                    {
                        Move = new Move(_board[message.Move.Origin.X, message.Move.Origin.Y], message.Move.Destination)
                    });
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
