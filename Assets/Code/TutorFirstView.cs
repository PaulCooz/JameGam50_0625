using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public sealed class TutorFirstView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup group;

        public UniTaskCompletionSource hideTcs { get; private set; }

        public void Show()
        {
            hideTcs = new();
            group.DOFade(1, 0.3f).OnComplete(() => { group.blocksRaycasts = true; });
        }

        public void Hide()
        {
            group.blocksRaycasts = false;
            group.DOFade(0, 0.3f).OnComplete(() => hideTcs.TrySetResult());
        }
    }
}