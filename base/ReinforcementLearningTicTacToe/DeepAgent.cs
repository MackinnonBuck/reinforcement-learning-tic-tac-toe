using Keras;
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

        private char _opponentTag;
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

        private BaseModel LoadModel()
        {
            string fileName = ModelFileName;

            if (File.Exists(fileName))
            {
                return BaseModel.LoadModel(fileName);
            }

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
        }

        public override string MakeMove(string state, char winner, bool learn)
        {
            if (learn)
            {
                Learn(state, winner);
            }

            return "TODO";
        }

        private string MakeOptimalMove(string state)
        {
            return "TODO";
        }

        private void Learn(string state, char winner)
        {
            
        }

        private NDarray CalculateTarget(string state, char winner)
        {
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
        }
    }
}
