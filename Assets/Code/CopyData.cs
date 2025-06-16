using System;
using System.Linq;
using UnityEngine;

namespace JamSpace
{
    [Serializable]
    public struct CopyData
    {
        public int width;
        public int height;

        [SerializeField]
        private Operator[] ops;

        public Operator this[int h, int w]
        {
            get => ops[h * width + w];
            set => ops[h * width + w] = value;
        }

        public CopyData(int width, int height, Operator[] ops)
        {
            this.width = width;
            this.height = height;
            this.ops = ops;
        }

        public CopyData(int width, int height, Operator[][] ops)
        {
            this.width = width;
            this.height = height;
            this.ops = ops.SelectMany(f => f).ToArray();
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

        public void SetAllEmpty()
        {
            ops = ops.Select(o => Operator.Empty).ToArray();
        }
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
        private const int Count = 8;

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