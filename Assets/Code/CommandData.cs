using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamSpace
{
    [Serializable]
    public struct CommandData
    {
        public int width;
        public int height;

        [SerializeField]
        private Operator[] flags;

        public Operator this[int h, int w]
        {
            get => flags[h * width + w];
            set => flags[h * width + w] = value;
        }

        public CommandData(int width, int height, Operator[][] flags)
        {
            this.width = width;
            this.height = height;
            this.flags = flags.SelectMany(f => f).ToArray();
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
                    Operator.XorLeft => output[j] ^ output[j - 1],
                    Operator.XorRight => output[j] ^ output[j + 1],
                    _ => output[j]
                };
            }

            return output;
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

        XorLeft,
        XorRight,
    }
}