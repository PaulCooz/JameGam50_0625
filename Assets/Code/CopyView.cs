using System;
using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public sealed class CopyView : MonoBehaviour
    {
        [SerializeField]
        private CopyGenSetting copyGenSetting;
        [SerializeField]
        private EnergyView energyView;
        [SerializeField]
        private HintsView hintsView;

        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private CopyPartView left, right;

        private LevelData _data;

        private void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        public void Show()
        {
            var rand = new System.Random(0);
            _data = copyGenSetting.Get(rand);

            energyView.Setup(_data);
            left.SetupCommandCopy(_data, true, hintsView.OnClickOp);
            right.SetupCommandCopy(_data, false, _ => { });
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1, 0.3f);
        }

        public void Check()
        {
            var input = new bool[_data.width];
            var allDone = true;
            for (var test = 0; test < (1 << _data.width); test++)
            {
                for (var j = 0; j < _data.width; j++)
                    input[j] = (test & (1 << j)) != 0;
                var lOut = left.Calc(input);
                var rOut = right.Calc(input);
                var success = true;
                for (var j = 0; j < _data.width && success; j++)
                    success &= lOut[j] == rOut[j];
                if (!success)
                {
                    left.SetInput(input);
                    right.SetInput(input);
                    allDone = false;
                    break;
                }
            }

            if (!allDone)
                Debug.Log("==> BAD");
            else
                Debug.Log("==> GOOD!");
        }

        public void Hide()
        {
            canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
            {
                energyView.Teardown();
                left.ClearViews();
                right.ClearViews();
                canvasGroup.blocksRaycasts = false;
            });
        }
    }
}