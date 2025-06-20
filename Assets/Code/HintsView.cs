using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class HintsView : MonoBehaviour
    {
        [SerializeField]
        private Toggle clear, info;

        private bool _one, _row, _col, _tryOne, _tryRow, _tryCol, _clear, _info;

        public void SetOneHint(bool value) => _one = value;
        public void SetRowHint(bool value) => _row = value;
        public void SetColHint(bool value) => _col = value;

        public void SetTryOneHint(bool value) => _tryOne = value;
        public void SetTryRowHint(bool value) => _tryRow = value;
        public void SetTryColHint(bool value) => _tryCol = value;

        public void SetClearHint(bool value) => _clear = value;
        public void SetInfoHint(bool value) => _info = value;

        public void OnClickOp(CopyOperatorView opView)
        {
            if (_one)
            {
                opView.HintOpen();
            }
            else if (_row)
            {
            }
            else if (_col)
            {
            }
            else if (_tryOne)
            {
            }
            else if (_tryRow)
            {
            }
            else if (_tryCol)
            {
            }
            else if (_clear)
            {
            }
            else if (_info)
            {
            }
        }
    }
}