using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CopyOperatorView : Button
    {
        private int _h, _w;
        private bool _hidden;
        private CopyData _data;

        [SerializeField]
        private TMP_Text tmp;

        public void Setup(int h, int w, CopyData data, bool hidden)
        {
            _h = h;
            _w = w;
            _data = data;
            _hidden = hidden;
        }

        public void Click()
        {
            if (_hidden)
                return;
            _data[_h, _w] = _data[_h, _w].MoveNext(_w, _data.width);
        }

        public void RefreshView()
        {
            tmp.text = _hidden
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
    }
}