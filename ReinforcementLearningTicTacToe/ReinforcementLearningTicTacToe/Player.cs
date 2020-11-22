namespace ReinforcementLearningTicTacToe
{
    internal abstract class Player
    {
        protected char Tag { get; }

        public Player(char tag)
        {
            Tag = tag;
        }

        public abstract string MakeMove(string state, char winner, bool learn);
    }
}