using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReinforcementLearningTicTacToe
{
    internal class DeepAgent
    {
        private readonly float _alpha = 0.5f;
        private readonly Sequential _model = new Sequential();
        private readonly float _expFactor;

        private char _tag;
        private char _opponentTag;
        private string _state;
        private string _prevState;

        public DeepAgent(char tag, float expFactor)
        {
            _tag = tag;
            _opponentTag = tag == 'X' ? 'O' : 'X';
            _expFactor = expFactor;

            _model.Add(new Dense(18, activation: "relu", input_dim: 9));
            _model.Add(new Dense(18, activation: "relu"));
            _model.Add(new Dense(1, activation: "linear"));
            _model.Compile(optimizer: "adam", loss: "mean_absolute_error", metrics: new[] { "accuracy" });

            _model.Summary();

            _state = null;
            _prevState = "123456789";
        }

        public string MakeMove(string state, char winner)
        {
            _state = state;

            if (winner != 'U')
            {
                return _state;
            }

            var p = np.random.uniform(
                new NDarray<float>(new[] { 0 }),
                new NDarray<float>(new[] { 1 }));

            string newState;

            if (p.GetData<float>()[0] < _expFactor) // TODO: Use C# random instead?????
            {
                newState = MakeOptimalMove(state);
            }
            else
            {
                var moves = state
                    .Where(c => int.TryParse(c.ToString(), out var _))
                    .ToArray();

                var idx = np.random.choice(new NDarray<char>(moves.ToArray())).GetData<char>()[0];
                newState = state.Replace(idx, _tag);
            }

            return newState;
        }

        public string MakeMoveAndLearn(string state, char winner)
        {
            Learn(state, winner);
            return MakeMove(state, winner);
        }

        private string MakeOptimalMove(string state)
        {
            var moves = state
                .Where(c => int.TryParse(c.ToString(), out var _))
                .ToArray();

            if (moves.Length == 1)
            {
                var move = moves[0];
                return state.Replace(move, _tag);
            }

            var tempStateList = new List<string>();
            var v = float.NegativeInfinity;

            foreach (var move in moves)
            {
                //var predictions = new List<NDarray>();
                var predictions = new NDarray<float>(new float[] { });
                var tempState = state.Replace(move, _tag);

                // TODO: Own func?
                var opponentMoves = tempState
                    .Where(c => int.TryParse(c.ToString(), out var _))
                    .ToArray();

                foreach (var opponentMove in opponentMoves)
                {
                    var tempStateOpponent = tempState.Replace(opponentMove, _opponentTag);
                    predictions.append(Predict(tempStateOpponent));
                }

                //predictions = predictions.Where(v => !(v is null)).ToList(); // TODO: Necessary?

                var vTemp = predictions.len == 0 ? 1.0f : np.min(predictions).GetData<float>()[0];

                if (vTemp > v)
                {
                    tempStateList = new List<string> { tempState };
                    v = vTemp;
                }
                else
                {
                    tempStateList.Add(tempState);
                }
            }

            if (tempStateList.Count == 0)
            {
                throw new InvalidOperationException("Oh no! The temp state was empty.");
            }

            return np.random.choice(new NDarray<string>(tempStateList.ToArray())).GetData<string>()[0];
        }

        private void Learn(string state, char winner)
        {
            var target = CalculateTarget(state, winner);

            Train(target, 10);

            _prevState = state;
        }

        private NDarray CalculateTarget(string state, char winner)
        {
            if (state.Contains(_tag))
            {
                var vs = Predict(_prevState);
                var r = CalcualteReward(winner);

                var vsTag = winner == 'U' ? Predict(state) : new NDarray<int>(new[] { 0 });

                return np.array(vs + _alpha * (r + vsTag - vs));
            }

            return null; // TODO: Is this a problem?
        }

        private float CalcualteReward(char winner)
        {
            if (winner == _tag)
            {
                return 1;
            }

            return winner switch
            {
                'U' => 0.0f,
                'T' => 0.5f,
                _ => -1.0f
            };
        }

        private NDarray<int> GetStateArray(string state) =>
            new NDarray<int>(state.Select(c => c switch
                {
                    'X' => 1,
                    'O' => -1,
                    _ => 0
                })
                .ToArray());

        private NDarray Predict(string state) =>
            _model.Predict(GetStateArray(state));

        private void Train(NDarray target, int epochs)
        {
            var xTrain = GetStateArray(_prevState);

            if (!(target is null))
            {
                _model.Fit(xTrain, target, epochs: epochs, verbose: 0);
            }
        }
    }
}
