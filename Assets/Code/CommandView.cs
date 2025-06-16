using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public sealed class CommandView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private CommandPartView left, right;

        public void Show()
        {
            left.SetupCommandResolving();
            right.SetupCommandResolving();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1, 0.3f);
        }

        public void Hide()
        {
            canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
            {
                left.ClearViews();
                right.ClearViews();
                canvasGroup.blocksRaycasts = false;
            });
        }
    }
}