using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CopyPartView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform inputParent;
        [SerializeField]
        private GridLayoutGroup gridParent;
        [SerializeField]
        private RectTransform outputParent;

        [SerializeField]
        private CommandInOutView inOutPrefab;
        [SerializeField]
        private CopyOperatorView operatorPrefab;

        private bool[] _currentInput;
        private LevelData _data;
        private List<CommandInOutView> _inputButtons;
        private List<CommandInOutView> _outputButtons;
        private List<CopyOperatorView> _gridButtons;

        public void SetupCommandCopy(LevelData data, bool hideOps, Func<CopyOperatorView, List<CopyOperatorView>, bool> onClickOp)
        {
            _data = data;
            if (!hideOps)
                _data.SetAllEmpty();

            var spacing =
                new Vector2((_data.width - 1) * gridParent.spacing.x, (_data.height - 1) * gridParent.spacing.y);
            var padding = new Vector2(gridParent.padding.horizontal, gridParent.padding.vertical);
            var size = ((RectTransform)gridParent.transform).rect.size;
            size = size - spacing - padding;
            size.x /= _data.width;
            size.y /= _data.height;
            size.x--;
            size.y--;
            gridParent.cellSize = size;

            _currentInput = new bool[_data.width];
            _inputButtons = new();
            _outputButtons = new();
            for (var i = 0; i < _data.width; i++)
            {
                _currentInput[i] = true;

                var inButton = Instantiate(inOutPrefab, inputParent);
                inButton.Setup(true);
                var index = i;
                inButton.RefreshView(_currentInput[index]);
                inButton.onClick.AddListener(() =>
                {
                    _currentInput[index] = !_currentInput[index];
                    _inputButtons[index].RefreshView(_currentInput[index]);
                    RefreshOutputView();
                });
                _inputButtons.Add(inButton);

                var outButton = Instantiate(inOutPrefab, outputParent);
                outButton.Setup(false);
                _outputButtons.Add(outButton);
            }

            _gridButtons = new();
            for (var i = 0; i < _data.height; i++)
            for (var j = 0; j < _data.width; j++)
            {
                var operatorView = Instantiate(operatorPrefab, gridParent.transform);
                operatorView.Setup(i, j, _data, hideOps);
                operatorView.RefreshView();
                operatorView.onClick.AddListener(() =>
                {
                    var shouldPerformClick = onClickOp(operatorView, _gridButtons);
                    if (shouldPerformClick)
                        operatorView.Click();
                    operatorView.RefreshView();
                    RefreshOutputView();
                });
                _gridButtons.Add(operatorView);
            }

            RefreshOutputView();
        }

        private void RefreshOutputView()
        {
            var output = _data.Calc(_currentInput);
            for (var j = 0; j < _data.width; j++)
                _outputButtons[j].RefreshView(output[j]);
        }

        public void ClearViews()
        {
            this.DestroyAll(_inputButtons);
            this.DestroyAll(_outputButtons);
            this.DestroyAll(_gridButtons);
        }

        public bool[] Calc(bool[] input) => _data.Calc(input);

        public void SetInput(bool[] input)
        {
            for (var i = 0; i < _currentInput.Length; i++)
            {
                _currentInput[i] = input[i];
                _inputButtons[i].RefreshView(_currentInput[i]);
            }

            RefreshOutputView();
        }
    }
}