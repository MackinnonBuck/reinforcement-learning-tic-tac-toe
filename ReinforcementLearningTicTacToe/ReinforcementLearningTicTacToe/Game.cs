using System;
using System.Linq;

namespace ReinforcementLearningTicTacToe
{
    internal class Game
    {
        private readonly Player _player1;
        private readonly Player _player2;

        private string _state;
        private char _winner;
        private char _turn;

        private Player _playerTurn;

        public Game(Player player1, Player player2)
        {
            _player1 = player1;
            _player2 = player2;

            _state = "123456789";
            _winner = 'U';
            _turn = 'X';

            _playerTurn = _player1;
        }

        public void PlayGame()
        {
            while (_winner == 'U')
            {
                if ( _playerTurn is HumanPlayer)
                {
                    PrintGame();
                }

                _state = PlayMove(false);
                _winner = FindWinner();
            }

            PrintGame();
        }

        private void PrintGame()
        {
            Console.WriteLine($"{_state[0]} | {_state[1]} | {_state[2]}");
            Console.WriteLine("--------------");
            Console.WriteLine($"{_state[3]} | {_state[4]} | {_state[5]}");
            Console.WriteLine("--------------");
            Console.WriteLine($"{_state[6]} | {_state[7]} | {_state[8]}\n");
        }

        public void PlayToLearn(int episodes)
        {
            for (int i = 0; i < episodes; i++)
            {
                while (_winner == 'U')
                {
                    _state = PlayMove(learn: true);
                    _winner = FindWinner();
                }

                _state = PlayMove(learn: true);
                _state = PlayMove(learn: true);
                _state = PlayMove(learn: true);
                _state = PlayMove(learn: true);

                if (i % 10 == 0)
                {
                    if (_player1 is DeepAgent deepAgent1)
                    {
                        deepAgent1.SaveModel();
                    }

                    if (_player2 is DeepAgent deepAgent2)
                    {
                        deepAgent2.SaveModel();
                    }
                }

                ResetGame();
            }
        }

        private string PlayMove(bool learn)
        {
            char nextTurn;
            Player currentPlayer;
            Player nextPlayer;

            if (_turn == 'X')
            {
                currentPlayer = _player1;
                nextTurn = 'O';
                nextPlayer = _player2;
            }
            else
            {
                currentPlayer = _player2;
                nextTurn = 'X';
                nextPlayer = _player1;
            }

            var newState = currentPlayer.MakeMove(_state, _winner, learn);

            _turn = nextTurn;
            _playerTurn = nextPlayer;

            return newState;
        }

        private char FindWinner()
        {
            if (_state.Count(c => int.TryParse(c.ToString(), out var _)) == 0)
            {
                return 'T';
            }

            var winner = new int[8, 3]
            {
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 },
                { 0, 3, 6 },
                { 1, 4, 7 },
                { 2, 5, 8 },
                { 0, 4, 8 },
                { 2, 4, 6 }
            };

            for (int i = 0; i < 8; i++)
            {
                string s = string.Join("", _state[winner[i, 0]], _state[winner[i, 1]], _state[winner[i, 2]]);

                if (s == "XXX")
                {
                    return 'X';
                }
                else if  (s == "OOO")
                {
                    return 'O';
                }
            }

            return 'U';
        }

        private void ResetGame()
        {
            _state = "123456789";
            _winner = 'U';
            _turn = 'X';
            _playerTurn = _player1;
        }
    }
}
