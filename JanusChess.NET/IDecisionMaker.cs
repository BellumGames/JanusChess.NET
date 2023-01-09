using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanusChess.NET
{
    public delegate void DecisionTakenHandler(object sender, DecisionTakenEventArgs e);

    public class DecisionTakenEventArgs : EventArgs
    {
        public Move Move { get; set; }
    }

    public interface IDecisionMaker
    {
        void StartThinkingAboutNextMove();
        event DecisionTakenHandler DecisionTaken;
    }
}
