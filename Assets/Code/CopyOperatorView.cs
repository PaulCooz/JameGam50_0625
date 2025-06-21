using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CopyOperatorView : Button
    {
        private State _state;
        private LevelData _data;

        [SerializeField]
        private TMP_Text tmp;

        public int row { get; private set; }
        public int col { get; private set; }

        public Operator? guess { get; private set; }
        public Operator oper
        {
            get => _data[row, col];
            set => _data[row, col] = value;
        }
        public bool alreadyHinted => _state is State.Hinted;

        public void Setup(int h, int w, LevelData data, bool hidden)
        {
            row = h;
            col = w;
            _data = data;
            _state = hidden ? State.Hidden : State.Usable;
        }

        public void Click()
        {
            if (_state is not State.Usable)
            {
                if (_state is State.Hidden)
                {
                    _state = State.Guessed;
                    guess = Operator.Empty;
                }
                else if (_state is State.Guessed)
                {
                    if (guess is OperatorExt.TheLastOne)
                    {
                        guess = null;
                        _state = State.Hidden;
                    }
                    else
                    {
                        guess = guess?.MoveNext(col, _data.width);
                    }
                }
            }
            else
            {
                oper = oper.MoveNext(col, _data.width);
            }
        }

        public void RefreshView()
        {
            if (_state is State.Hidden)
            {
                tmp.text = "?";
            }
            else if (_state is State.Guessed)
            {
                tmp.text = guess!.Value switch
                {
                    Operator.Empty => " ",
                    Operator.Not => "!",
                    Operator.AndLeft => "-&",
                    Operator.AndRight => "&-",
                    Operator.OrLeft => "-|",
                    Operator.OrRight => "|-",
                    _ => throw new ArgumentOutOfRangeException()
                };
                tmp.text += "?";
            }
            else
            {
                tmp.text = oper switch
                {
                    Operator.Empty => " ",
                    Operator.Not => "!",
                    Operator.AndLeft => "-&",
                    Operator.AndRight => "&-",
                    Operator.OrLeft => "-|",
                    Operator.OrRight => "|-",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public void HintOpen()
        {
            _state = State.Hinted;
            RefreshView();
        }

        private enum State
        {
            Usable,
            Hidden,
            Hinted,
            Guessed,
        }
    }
}