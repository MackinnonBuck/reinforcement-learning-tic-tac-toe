namespace ReinforcementLearningTicTacToe
{
    internal class Game
    {
        private readonly DeepAgent _player1;
        private readonly DeepAgent _player2;

        private string _state;
        private char _winner;
        private char _turn;

        private DeepAgent _playerTurn;

        private int _xWins;
        private int _oWins;
        private int _tieWins;
        private int _totalWins;

        public Game()
        {
            _player1 = new DeepAgent('X', 0.8f);
            _player2 = new DeepAgent('O', 0.8f);

            _state = "123456789";
            _winner = 'U';
            _turn = 'X';

            _playerTurn = _player1;

            _xWins = 0;
            _oWins = 0;
            _tieWins = 0;
            _totalWins = 0;
        }

        private void PlayToLearn(int episodes)
        {
            for (int i = 0; i < episodes; i++)
            {
                while (_winner == 'U')
                {
                    _state = PlayMove(learn: true);
                    _winner = FindWinner();
                }
            }
        }

        private string PlayMove(bool learn)
        {
            DeepAgent currentPlayer;
            char nextTurn;
            DeepAgent nextPlayer;

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

            var newState = learn ?
                currentPlayer.MakeMoveAndLearn(_state, _winner) :
                currentPlayer.MakeMove(_state, _winner);

            _turn = nextTurn;
            _playerTurn = nextPlayer;

            return newState;
        }

        private char FindWinner()
        {
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

                // TODO: You are here.
            }
        }
    }
}
