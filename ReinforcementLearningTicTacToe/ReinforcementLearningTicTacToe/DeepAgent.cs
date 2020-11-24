using Keras.Layers;
using Keras.Models;
using Numpy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReinforcementLearningTicTacToe
{
    internal class DeepAgent : Player
    {
        private readonly float _alpha = 0.5f;
        private readonly BaseModel _model;
        private readonly float _expFactor;
        private readonly char _opponentTag;

        private readonly Random _random = new Random();

        private string _state;
        private string _prevState;

        private string ModelFileName => $"model_values{Tag}.h5";

        public DeepAgent(char tag, float expFactor) : base(tag)
        {
            _opponentTag = tag == 'X' ? 'O' : 'X';
            _expFactor = expFactor;

            _model = LoadModel();

            _model.Summary();

            _state = null;
            _prevState = "123456789";
        }

        // This logic is mostly the same
        private BaseModel LoadModel()
        {
            string fileName = ModelFileName;

            if (File.Exists(fileName))
            {
                return BaseModel.LoadModel(fileName);
            }

            // Use the model we've already made.
            // input_dim will be the number of floats we're training on.
            // input_shape should still be 1 (or 3, if we're always working with 3D vectors? but probably 1).
            var model = new Sequential();
            model.Add(new Dense(18, activation: "relu", input_dim: 9, input_shape: 1));
            model.Add(new Dense(18, activation: "relu"));
            model.Add(new Dense(1, activation: "linear"));
            model.Compile(optimizer: "adam", loss: "mean_absolute_error", metrics: new[] { "accuracy" });

            return model;
        }

        public void SaveModel()
        {
            _model.Save(ModelFileName, true);
            Console.WriteLine("-------- Model has been saved! --------");
        }

        // This calculates the skill we want to perform.
        // _state gets translated to skill ID.
        public override string MakeMove(string state, char winner, bool learn)
        {
            if (learn)
            {
                Learn(state, winner);
            }

            _state = state;

            if (winner != 'U')
            {
                return _state;
            }

            // Exploration factor stays in
            var p = (float)_random.NextDouble();

            string newState;

            if ((float)p < _expFactor)
            {
                // This logic stays about the same.
                newState = MakeOptimalMove(state);
            }
            else
            {
                // Just choose a random number between 0 and the max skill ID.
                var moves = state
                    .Where(c => int.TryParse(c.ToString(), out var _))
                    .Select(c => c.ToString())
                    .ToArray();

                var idx = moves[_random.Next(0, moves.Length)][0];
                newState = state.Replace(idx, Tag);
            }

            return newState;
        }

        private string MakeOptimalMove(string state)
        {
            var moves = state
                .Where(c => int.TryParse(c.ToString(), out var _))
                .ToArray();

            if (moves.Length == 1)
            {
                var move = moves[0];
                return state.Replace(move, Tag);
            }

            var tempStateList = new List<string>();
            var v = float.NegativeInfinity;

            foreach (var move in moves)
            {
                var predictions = new List<float[]>();
                var tempState = state.Replace(move, Tag);

                var opponentMoves = tempState
                    .Where(c => int.TryParse(c.ToString(), out var _))
                    .ToArray();

                foreach (var opponentMove in opponentMoves)
                {
                    var tempStateOpponent = tempState.Replace(opponentMove, _opponentTag);
                    var prediction = Predict(tempStateOpponent);
                    predictions.Add(prediction.GetData<float>());
                }

                var vTemp = predictions.Count == 0 ? 1.0f : predictions.SelectMany(p => p).Min();

                if (vTemp > v)
                {
                    tempStateList = new List<string> { tempState };
                    v = vTemp;
                }
                else if (vTemp == v)
                {
                    tempStateList.Add(tempState);
                }
            }

            if (tempStateList.Count == 0)
            {
                throw new InvalidOperationException("Oh no! The temp state was empty.");
            }

            return tempStateList[_random.Next(0, tempStateList.Count)];
        }

        private void Learn(string state, char winner)
        {
            var target = CalculateTarget(state, winner);

            Train(target, 10);

            _prevState = state;
        }

        private NDarray CalculateTarget(string state, char winner)
        {
            if (state.Contains(Tag))
            {
                var vs = Predict(_prevState);
                var r = CalculateReward(winner);

                var vsTag = winner == 'U' ? Predict(state) : new NDarray<int>(new[] { 0 });

                return np.array(vs + _alpha * (r + vsTag - vs));
            }

            return null;
        }

        private float CalculateReward(char winner)
        {
            if (winner == Tag)
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
            np.array(state.Select(c => c switch
                {
                    'X' => 1,
                    'O' => -1,
                    _ => 0
                })
                .ToArray());

        private NDarray Predict(string state)
        {
            var stateArray = GetStateArray(state);
            return _model.Predict(stateArray);
        }

        private void Train(NDarray target, int epochs)
        {
            // Could this be something as simple as:
            // var result _model.Predict(state)
            // _model.Fit(state, result)

            var xTrain = GetStateArray(_prevState);

            if (!(target is null))
            {
                _model.Fit(xTrain, target, epochs: epochs, verbose: 0);
            }
        }
    }
}
