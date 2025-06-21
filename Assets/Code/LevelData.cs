using System;
using System.Linq;
using UnityEngine;

namespace JamSpace
{
    [Serializable]
    public class LevelData
    {
        private int _currentEnergy;

        [SerializeField]
        private Operator[] ops;

        public int width { get; }
        public int height { get; }

        public int totalEnergy { get; }
        public int currentEnergy
        {
            get => _currentEnergy;
            set => _currentEnergy = Math.Clamp(value, 0, totalEnergy);
        }

        public Operator this[int h, int w]
        {
            get => ops[h * width + w];
            set => ops[h * width + w] = value;
        }

        public LevelData(int totalEnergy, int width, int height, Operator[] ops)
        {
            this.totalEnergy = totalEnergy;
            this.currentEnergy = this.totalEnergy;

            this.width = width;
            this.height = height;
            this.ops = ops;
        }

        public LevelData(LevelData copy)
        {
            totalEnergy = copy.totalEnergy;
            currentEnergy = totalEnergy;

            width = copy.width;
            height = copy.height;
            ops = copy.ops;
        }

        public bool[] Calc(bool[] input)
        {
            if (input.Length != width)
                throw new("wrong input width");

            var output = new bool[width];
            for (var j = 0; j < width; j++)
                output[j] = input[j];

            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
            {
                output[j] = this[i, j] switch
                {
                    Operator.Not => !output[j],
                    Operator.AndLeft => output[j] & output[j - 1],
                    Operator.AndRight => output[j] & output[j + 1],
                    Operator.OrLeft => output[j] | output[j - 1],
                    Operator.OrRight => output[j] | output[j + 1],
                    _ => output[j]
                };
            }

            return output;
        }

        public void SetAllEmpty() { ops = ops.Select(o => Operator.Empty).ToArray(); }
    }

    public enum Operator
    {
        Empty,

        Not,

        AndLeft,
        AndRight,

        OrLeft,
        OrRight,
    }

    public static class OperatorExt
    {
        public const Operator TheLastOne = Operator.OrRight;

        public static readonly int Count = Enum.GetNames(typeof(Operator)).Length;

        public static Operator GetRandNonEmpty(System.Random rand, int j, int width)
        {
            var op = (Operator)rand.Next(0, Count);
            move:
            op = MoveNext(op, j, width);
            if (op == Operator.Empty)
                goto move;
            return op;
        }

        public static Operator MoveNext(this Operator op, int j, int width)
        {
            inc:
            var value = (int)op;
            op = value == (int)Operator.OrRight ? Operator.Empty : (Operator)(value + 1);

            switch (op)
            {
                case Operator.AndLeft or Operator.OrLeft when j == 0:
                case Operator.AndRight or Operator.OrRight when j == width - 1:
                    goto inc;
            }

            return op;
        }
    }
}