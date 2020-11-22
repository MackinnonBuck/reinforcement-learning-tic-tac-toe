﻿namespace ReinforcementLearningTicTacToe
{
    internal class Program
    {
        private static void Main()
        {
            // Training
            DeepAgent player1 = new DeepAgent('X', 1.0f);
            DeepAgent player2 = new DeepAgent('O', 0.0f);

            var game = new Game(player1, player2);
            game.PlayToLearn(50);
            player1.SaveModel();
            player2.SaveModel();

            // Playing
            //DeepAgent player1 = new DeepAgent('X', 0.8f);
            //HumanPlayer player2 = new HumanPlayer('O');

            //var game = new Game(player1, player2);
            //game.PlayGame();

            //player1.SaveModel();
        }
    }
}