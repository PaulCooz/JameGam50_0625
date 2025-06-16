using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CommandOperatorView : Button
    {
        private int _h, _w;
        private CommandData _data;

        [SerializeField]
        private TMP_Text tmp;

        public void Setup(int h, int w, CommandData data)
        {
            _h = h;
            _w = w;
            _data = data;
        }

        public void Click()
        {
            var op = _data[_h, _w];
            if (op == Operator.Empty)
                _data[_h, _w] = Operator.Not;
            else if (op == Operator.Not)
                _data[_h, _w] = Operator.Empty;
        }

        public void RefreshView()
        {
            var op = _data[_h, _w];
            tmp.text = op.ToString();
        }
    }
}