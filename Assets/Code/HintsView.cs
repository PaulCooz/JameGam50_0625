using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class HintsView : MonoBehaviour
    {
        [SerializeField]
        private ToggleGroup toggleGroup;
        [SerializeField]
        private List<HintToggle> toggles;

        private EnergyView _energyView;

        private Dictionary<Hint, (bool isOn, Func<CopyOperatorView, IReadOnlyList<CopyOperatorView>, float> perform)>
            _hint;

        private LevelData _data;
        private float totalEnergy => _data.totalEnergy;
        private float totalSize => _data.height * _data.width;

        private float GetEnForN(double forOne, int n) =>
            (float)Math.Max(forOne * Utils.SexyPow(0.99, Mathf.Max(n, 1) - 1), 1.0);

        private float enForOne => Mathf.Max(totalSize > 1f ? totalEnergy / (totalSize - 1f) : 1f, 1f);

        private float GetEnForOne(int n) => GetEnForN(enForOne, n);

        private float enForRow => GetEnForOne(_data.width);
        private float enForCol => GetEnForOne(_data.height);

        private float enForRightTry => Mathf.Max(enForOne / 4f, 1f);
        private float enForWrongTry => Mathf.Max(totalEnergy / totalSize, 1f);

        private float enForRightTryRow => GetEnForN(enForRightTry, _data.width);
        private float enForWrongTryRow => GetEnForN(enForWrongTry, _data.width);
        private float enForRightTryCol => GetEnForN(enForRightTry, _data.height);
        private float enForWrongTryCol => GetEnForN(enForWrongTry, _data.height);

        private float enForClear => GetEnForOne(40 * _data.width * _data.height / 100);
        private float enForInfo => Mathf.Max(totalEnergy / 8f, 1f);

        private void Awake()
        {
            _hint = new()
            {
                [Hint.One] = (false, OpenOne),
                [Hint.Row] = (false, OpenRow),
                [Hint.Col] = (false, OpenCol),
                [Hint.TryOne] = (false, TryOne),
                [Hint.TryRow] = (false, TryRow),
                [Hint.TryCol] = (false, TryCol),
                [Hint.Clear] = (false, Clear),
                [Hint.Info] = (false, Info),
            };
            foreach (var t in toggles)
            {
                var hint = t.hint;
                t.toggle.onValueChanged.AddListener(value =>
                {
                    var x = _hint[hint];
                    x.isOn = value;
                    _hint[hint] = x;
                });
            }
        }

        public void Setup(EnergyView energyView, LevelData data)
        {
            _energyView = energyView;
            _data = data;
        }

        public bool OnClickOp(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> all)
        {
            var shouldPerformClick = true;
            foreach (var (_, t) in _hint)
            {
                if (t.isOn)
                {
                    var energySub = t.perform(opView, all);
                    _energyView.SubtractEnergy(Mathf.Max(Mathf.RoundToInt(energySub), 1));
                    shouldPerformClick = false;
                    break;
                }
            }

            DeactivateAllToggles();
            return shouldPerformClick;
        }

        private void DeactivateAllToggles() => toggleGroup.ActiveToggles().ToList().ForEach(t => t.isOn = false);

        private float OpenOne(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            opView.HintOpen();
            return enForOne;
        }

        private float OpenRow(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            foreach (var view in allViews)
                if (view.row == opView.row && !view.alreadyHinted)
                    view.HintOpen();
            return enForRow;
        }

        private float OpenCol(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            foreach (var view in allViews)
                if (view.col == opView.col && !view.alreadyHinted)
                    view.HintOpen();
            return enForCol;
        }

        private float TryOne(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            var energy = enForRightTry;
            if (opView.guess.HasValue && opView.guess.Value == opView.oper)
                OpenOne(opView, allViews);
            else
                energy = enForWrongTry;
            return energy;
        }

        private float TryRow(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            var open = true;
            foreach (var view in allViews)
            {
                if (view.row == opView.row && !view.alreadyHinted)
                    open &= view.guess.HasValue && view.guess.Value == view.oper;
            }

            var energy = enForRightTryRow;
            if (open)
                OpenRow(opView, allViews);
            else
                energy = enForWrongTryRow;
            return energy;
        }

        private float TryCol(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            var open = true;
            foreach (var view in allViews)
            {
                if (view.col == opView.col && !view.alreadyHinted)
                    open &= view.guess.HasValue && view.guess.Value == view.oper;
            }

            var energy = enForRightTryCol;
            if (open)
                OpenCol(opView, allViews);
            else
                energy = enForWrongTryCol;
            return energy;
        }

        private float Clear(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            foreach (var view in allViews)
                if (view.oper is Operator.Empty && !view.alreadyHinted)
                    view.HintOpen();
            return enForClear;
        }

        private float Info(CopyOperatorView opView, IReadOnlyList<CopyOperatorView> allViews)
        {
            var d = new Dictionary<Operator, int>();
            foreach (var view in allViews)
            {
                var uniqueOper = view.oper switch
                {
                    Operator.Empty => Operator.Empty,
                    Operator.Not => Operator.Not,
                    Operator.AndLeft => Operator.AndLeft,
                    Operator.AndRight => Operator.AndLeft,
                    Operator.OrLeft => Operator.OrLeft,
                    Operator.OrRight => Operator.OrLeft,
                    _ => throw new ArgumentOutOfRangeException()
                };

                d.TryAdd(uniqueOper, 0);
                d[uniqueOper]++;
            }

            var sb = new StringBuilder();
            sb.Append("There are ");
            const string separator = ", ";
            var appendedSmth = false;
            foreach (var (op, count) in d)
            {
                if (op is Operator.Empty)
                    continue;
                var oper = op switch
                {
                    Operator.Not => "!",
                    Operator.AndLeft => "&",
                    Operator.OrLeft => "|",
                    _ => throw new ArgumentOutOfRangeException()
                };
                sb.Append($"'{oper}'x{count}{separator}");
                appendedSmth = true;
            }

            if (appendedSmth)
                sb.Remove(sb.Length - separator.Length, separator.Length);
            else
                sb.Append("nothing to do  :P");

            MessageView.Push(sb.ToString());

            _data.usedInfoHint = true;
            return enForInfo;
        }

        [Serializable]
        public struct HintToggle
        {
            public Hint hint;
            public Toggle toggle;
            public TMP_Text costTmp;
        }

        public enum Hint
        {
            One,
            Row,
            Col,
            TryOne,
            TryRow,
            TryCol,
            Clear,
            Info,
        }
    }
}