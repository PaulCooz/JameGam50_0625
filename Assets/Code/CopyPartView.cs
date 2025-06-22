using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CopyPartView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform backParent;
        [SerializeField]
        private LineView linePrefab;

        private List<LineView> _lines = new();

        [SerializeField]
        private GridLayoutGroup inputParent;
        [SerializeField]
        private GridLayoutGroup gridParent;
        [SerializeField]
        private GridLayoutGroup outputParent;

        [SerializeField]
        private CommandInOutView inOutPrefab;
        [SerializeField]
        private CopyOperatorView operatorPrefab;

        [SerializeField]
        private UnityEvent anyClick;

        private bool[] _currentInput;
        private LevelData _data;
        private bool _isPlayerSide;
        private List<CommandInOutView> _inputButtons;
        private List<CommandInOutView> _outputButtons;
        private List<CopyOperatorView> _gridButtons;

        public void SetupCommandCopy(
            LevelData data, bool playerSide, Func<CopyOperatorView, List<CopyOperatorView>, bool> onClickOp)
        {
            _data = data;
            _isPlayerSide = playerSide;

            const float minSpace = 10;
            var inRTr = (RectTransform)inputParent.transform;
            var freeForCellInH = inRTr.rect.height - inputParent.padding.vertical;
            var freeForCellInW =
                (inRTr.rect.width - inputParent.padding.horizontal - (_data.width - 1) * minSpace) /
                _data.width;

            var gridRTr = (RectTransform)gridParent.transform;
            var freeForCellGridH =
                (gridRTr.rect.height - gridParent.padding.vertical - (_data.height - 1) * minSpace) / _data.height;
            var freeForCellGridW =
                (gridRTr.rect.width - gridParent.padding.horizontal - (_data.width - 1) * minSpace) / _data.width;

            var minSide =
                Mathf.Min(Mathf.Min(freeForCellInH, freeForCellGridH), Mathf.Min(freeForCellInW, freeForCellGridW));

            SetupGrid(inputParent, minSide, _data.width, 1);
            SetupGrid(outputParent, minSide, _data.width, 1);
            SetupGrid(gridParent, minSide, _data.width, _data.height);

            _currentInput = new bool[_data.width];
            _inputButtons = new();
            _outputButtons = new();
            for (var i = 0; i < _data.width; i++)
            {
                _currentInput[i] = true;

                var inButton = Instantiate(inOutPrefab, inputParent.transform);
                inButton.Setup(true);
                var index = i;
                inButton.RefreshView(_currentInput[index]);
                inButton.onClick.AddListener(() =>
                {
                    anyClick.Invoke();

                    _currentInput[index] = !_currentInput[index];
                    _inputButtons[index].RefreshView(_currentInput[index]);
                    RefreshOutputView();
                });
                _inputButtons.Add(inButton);

                var outButton = Instantiate(inOutPrefab, outputParent.transform);
                outButton.Setup(false);
                _outputButtons.Add(outButton);
            }

            _gridButtons = new();
            for (var i = 0; i < _data.height; i++)
            for (var j = 0; j < _data.width; j++)
            {
                var operatorView = Instantiate(operatorPrefab, gridParent.transform);
                operatorView.Setup(i, j, _data, _isPlayerSide);
                operatorView.RefreshView();
                operatorView.onClick.AddListener(() =>
                {
                    anyClick.Invoke();

                    var shouldPerformClick = onClickOp(operatorView, _gridButtons);
                    if (shouldPerformClick)
                        operatorView.Click();
                    operatorView.RefreshView();
                    RefreshOutputView();
                });
                _gridButtons.Add(operatorView);
            }

            UniTask.NextFrame().ContinueWith(RefreshOutputView).Forget();
        }

        private void SetupGrid(GridLayoutGroup grid, float side, int w, int h)
        {
            var rt = (RectTransform)grid.transform;
            var spacing = grid.spacing;

            spacing.x = spacing.y = 0;
            if (w > 1)
                spacing.x = (rt.rect.width - grid.padding.horizontal - w * side) / (w + 1);
            if (h > 1)
                spacing.y = (rt.rect.height - grid.padding.vertical - h * side) / (h + 1);

            grid.spacing = spacing;
            grid.cellSize = new(side, side);
        }

        private void RefreshOutputView()
        {
            var output = _data.Calc(_currentInput);
            for (var j = 0; j < _data.width; j++)
                _outputButtons[j].RefreshView(output[j]);

            this.DestroyAll(_lines);
            for (var j = 0; j < _inputButtons.Count; j++)
            {
                var line = Instantiate(linePrefab, backParent);
                line.Setup(
                    (RectTransform)_inputButtons[j].transform,
                    (RectTransform)_gridButtons[0 * _data.width + j].transform
                );

                _lines.Add(line);
            }

            for (var i = 0; i < _data.height; i++)
            for (var j = 0; j < _data.width; j++)
            {
                var curr = _gridButtons[i * _data.width + j];
                if (i + 1 < _data.height)
                {
                    var line = Instantiate(linePrefab, backParent);
                    line.Setup(
                        (RectTransform)curr.transform,
                        (RectTransform)_gridButtons[(i + 1) * _data.width + j].transform
                    );
                    _lines.Add(line);
                }

                if (!_isPlayerSide && !_data.GetHint(i, j) && !curr.guess.HasValue)
                    continue;

                var oper = _isPlayerSide || _data.GetHint(i, j) ? _data[i, j] : curr.guess!.Value;
                switch (oper)
                {
                    case Operator.Empty:
                    case Operator.Not:
                        break;
                    case Operator.AndLeft:
                    case Operator.OrLeft:
                    {
                        var crossLine = Instantiate(linePrefab, backParent);
                        crossLine.Setup(
                            (RectTransform)_gridButtons[i * _data.width + (j - 1)].transform,
                            (RectTransform)curr.transform
                        );
                        _lines.Add(crossLine);
                        break;
                    }
                    case Operator.AndRight:
                    case Operator.OrRight:
                    {
                        var crossLine = Instantiate(linePrefab, backParent);
                        crossLine.Setup(
                            (RectTransform)curr.transform,
                            (RectTransform)_gridButtons[i * _data.width + (j + 1)].transform
                        );
                        _lines.Add(crossLine);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            for (var j = 0; j < _outputButtons.Count; j++)
            {
                var line = Instantiate(linePrefab, backParent);
                line.Setup(
                    (RectTransform)_gridButtons[(_data.height - 1) * _data.width + j].transform,
                    (RectTransform)_outputButtons[j].transform
                );

                _lines.Add(line);
            }
        }

        public void ShowAll()
        {
            foreach (var opView in _gridButtons)
                opView.ForceShow();
        }

        public void ClearViews()
        {
            this.DestroyAll(_lines);
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