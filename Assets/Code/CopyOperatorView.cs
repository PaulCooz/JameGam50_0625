using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CopyOperatorView : Button
    {
        private int _h, _w;
        private State _state;
        private LevelData _data;

        [SerializeField]
        private TMP_Text tmp;

        public void Setup(int h, int w, LevelData data, bool hidden)
        {
            _h = h;
            _w = w;
            _data = data;
            _state = hidden ? State.Hidden : State.Usable;
        }

        public void Click()
        {
            if (_state is not State.Usable)
                return;
            _data[_h, _w] = _data[_h, _w].MoveNext(_w, _data.width);
        }

        public void RefreshView()
        {
            tmp.text = _state is State.Hidden
                ? "?"
                : _data[_h, _w] switch
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
        }
    }
}