﻿using System;
using System.Collections.Generic;
using System.Text;
using SiaNet.Engine;
using SiaNet.Layers;

namespace SiaNet.Optimizers
{
    public class RMSProp : BaseOptimizer
    {
        public float Rho { get; set; }

        public float Epsilon { get; set; }

        private Dictionary<string, Tensor> accumulators;

        public RMSProp(float lr = 0.001f, float rho = 0.9f, float decayRate = 0, float epsilon = 1e-07f)
            : base(lr, "rmsprop")
        {
            DecayRate = decayRate;
            Rho = rho;
            Epsilon = epsilon;
            accumulators = new Dictionary<string, Tensor>();
        }

        public override void Update(int iteration, BaseLayer layer)
        {
            if(DecayRate > 0)
            {
                LearningRate = LearningRate * (1 / (1 + DecayRate * iteration));
            }

            foreach (var item in layer.Params)
            {
                var param = item.Value;
                if (!accumulators.ContainsKey(param.Name))
                {
                    accumulators[param.Name] = K.Constant(0, param.Data.Shape);
                }

                accumulators[param.Name] = Rho * accumulators[param.Name] + (1 - Rho) * K.Square(param.Grad);

                param.Data = param.Data - (LearningRate * param.Grad / (K.Sqrt(accumulators[param.Name]) + Epsilon));

                param.ApplyConstraint();
            }
        }
    }
}