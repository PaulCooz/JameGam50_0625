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
        private bool _playerSide;

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

        public void Setup(int h, int w, LevelData data, bool playerSide)
        {
            row = h;
            col = w;
            _data = data;
            _playerSide = playerSide;

            if (_playerSide)
                _state = State.Usable;
            else
                _state = _data.GetHint(row, col) ? State.Hinted : State.Hidden;
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
                    var prev = guess ?? Operator.Empty;
                    guess = guess?.MoveNext(col, _data.width);

                    if (prev > guess)
                    {
                        guess = null;
                        _state = State.Hidden;
                    }
                }
            }
            else
            {
                oper = oper.MoveNext(col, _data.width);
            }
        }

        public void ForceShow()
        {
            _state = State.Hinted;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_state is State.Hidden)
            {
                tmp.text = "??";
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
            _data.SetHint(row, col, true);
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