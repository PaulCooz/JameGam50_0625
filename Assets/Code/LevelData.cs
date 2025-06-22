using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace JamSpace
{
    public sealed class LevelData
    {
        private int _currentEnergy;
        private readonly List<Operator> _ops;

        private bool _usedInfoHint;
        private readonly List<bool> _hinted;

        public int number { get; }
        public int width { get; }
        public int height { get; }

        public int totalEnergy { get; }
        public int currentEnergy
        {
            get => _currentEnergy;
            set
            {
                _currentEnergy = Math.Clamp(value, 0, totalEnergy);
                OnAnyChange?.Invoke();
            }
        }

        public bool usedInfoHint
        {
            get => _usedInfoHint;
            set
            {
                _usedInfoHint = value;
                OnAnyChange?.Invoke();
            }
        }

        public Operator this[int h, int w]
        {
            get => _ops[h * width + w];
            set
            {
                _ops[h * width + w] = value;
                OnAnyChange?.Invoke();
            }
        }

        public event Action OnAnyChange;

        public LevelData(int number, int totalEnergy, int width, int height, List<Operator> ops)
        {
            this.number = number;

            this.totalEnergy = totalEnergy;
            this.currentEnergy = this.totalEnergy;

            this.width = width;
            this.height = height;
            this._ops = ops;

            this.usedInfoHint = false;
            this._hinted = _ops.Select(_ => false).ToList();
        }

        public LevelData(LevelData copy)
        {
            number = copy.number;

            totalEnergy = copy.totalEnergy;
            currentEnergy = totalEnergy;

            width = copy.width;
            height = copy.height;
            _ops = new(copy._ops);

            usedInfoHint = copy.usedInfoHint;
            _hinted = new(copy._hinted);
        }

        [Preserve, JsonConstructor]
        public LevelData(
            int number, int totalEnergy, int currentEnergy, int width, int height, List<Operator> operators,
            bool usedInfoHint, List<bool> hinted
        ) : this(number, totalEnergy, width, height, operators)
        {
            this._currentEnergy = currentEnergy;
            this.usedInfoHint = usedInfoHint;
            this._hinted = new(hinted);
        }

        public string ToJson() => new JObject
        {
            { "number", number },
            { "totalEnergy", totalEnergy },
            { "currentEnergy", currentEnergy },
            { "width", width },
            { "height", height },
            { "operators", JArray.FromObject(_ops) },
            { "usedInfoHint", usedInfoHint },
            { "hinted", JArray.FromObject(_hinted) },
        }.ToString(Formatting.None);

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

        public bool GetHint(int h, int w) { return _hinted[h * width + w]; }

        public void SetHint(int h, int w, bool value)
        {
            _hinted[h * width + w] = value;
            OnAnyChange?.Invoke();
        }

        public void SetAllEmpty()
        {
            for (var i = 0; i < _ops.Count; i++)
                _ops[i] = Operator.Empty;
            OnAnyChange?.Invoke();
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