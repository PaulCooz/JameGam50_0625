using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class CommandPartView : MonoBehaviour
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
        private CommandOperatorView operatorPrefab;

        private bool[] _currentInput;
        private CommandData _data;
        private List<CommandInOutView> _inputButtons;
        private List<CommandInOutView> _outputButtons;
        private List<CommandOperatorView> _gridButtons;

        public void SetupCommandResolving()
        {
            _data = new(3, 2, new[]
            {
                new[] { Operator.Not, Operator.Empty, Operator.AndLeft },
                new[] { Operator.OrRight, Operator.Empty, Operator.Empty },
            });

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
                operatorView.Setup(i, j, _data);
                operatorView.RefreshView();
                operatorView.onClick.AddListener(() =>
                {
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
            Clear(_inputButtons);
            Clear(_outputButtons);
            Clear(_gridButtons);
            return;

            static void Clear<T>(List<T> objects) where T : Component
            {
                foreach (var b in objects)
                    Destroy(b.gameObject);
                objects.Clear();
            }
        }
    }
}