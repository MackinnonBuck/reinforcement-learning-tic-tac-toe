using System;

namespace ReinforcementLearningTicTacToe
{
    internal class HumanPlayer : Player
    {
        public HumanPlayer(char tag) : base(tag)
        {
        }

        public override string MakeMove(string state, char winner, bool learn)
        {
            Console.WriteLine("Choose move 1-9: ");

            string input;

            while (true)
            {
                input = Console.ReadLine();

                if (int.TryParse(input, out var move))
                {
                    if (move >= 1 && move <= 9 && state.Contains(input))
                    {
                        break;
                    }
                }

                Console.WriteLine("Invalid Move.");
                Console.WriteLine("Choose move 1-9: ");       
            }

            return state.Replace(input[0], Tag);
        }
    }
}