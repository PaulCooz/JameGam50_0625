using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace JamSpace
{
    public sealed class MessageView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text tmp;

        [SerializeField]
        private float timeScale = 70f;

        private Vector3 _parentLBL, _parentLTR, _parentWBL, _parentWTR;

        private Tween _anim;
        public bool inProcess { get; private set; }

        private readonly Queue<(string msg, MType type)> _queueLocal = new();
        private static readonly Queue<(string msg, MType type)> Queue = new();

        public static void Push(string message, MType type = MType.Info) => Queue.Enqueue((message, type));

        public void PushLocal(string message) => _queueLocal.Enqueue((message, MType.Info));

        private void Start()
        {
            var parent = (RectTransform)tmp.rectTransform.parent;
            var matL2W = parent.localToWorldMatrix;
            var r = parent.rect;
            _parentLBL = new(r.x, r.y, 0.0f);
            _parentLTR = new(r.xMax, r.yMax, 0.0f);
            _parentWBL = matL2W.MultiplyPoint(_parentLBL);
            _parentWTR = matL2W.MultiplyPoint(_parentLTR);

            tmp.alpha = 0;
        }

        private double _lastUpdate;

        private void Update()
        {
            var time = Time.timeAsDouble;
            if (time - _lastUpdate > 1)
            {
                _lastUpdate = time;
                TryDequeue(_queueLocal);
                TryDequeue(Queue);
            }
        }

        private void TryDequeue(Queue<(string msg, MType type)> queue)
        {
            if (queue.Count > 0)
            {
                if (inProcess)
                {
                    SpeedUp();
                }
                else
                {
                    var (m, t) = queue.Dequeue();
                    if (string.IsNullOrEmpty(m))
                        SpeedUp();
                    else
                        PushMessageAsync(m).Forget();
                }
            }
        }

        private void SpeedUp() => _anim?.DOTimeScale(100, 0.9f);

        private async UniTask PushMessageAsync(string msg)
        {
            _anim.TryKill(true);

            inProcess = true;
            tmp.alpha = 0f;

            tmp.text = msg;

            await UniTask.NextFrame();

            var sz = tmp.rectTransform.sizeDelta;
            sz.x = tmp.preferredWidth;
            tmp.rectTransform.sizeDelta = sz;

            await UniTask.NextFrame();

            var middleRight = new Vector3(_parentWTR.x, (_parentWTR.y + _parentWBL.y) / 2f, 0);
            tmp.rectTransform.position = middleRight;
            var renderedWidth = tmp.renderedWidth;
            tmp.rectTransform.localPosition += new Vector3(renderedWidth / 2f, 0, 0);

            tmp.alpha = 1f;

            var allHideX = _parentLBL.x - renderedWidth / 2f;
            _anim = tmp.rectTransform
                .DOLocalMoveX(allHideX, Mathf.Abs(tmp.rectTransform.localPosition.x - allHideX) / timeScale)
                .SetEase(Ease.Linear)
                .OnComplete(() => inProcess = false);
        }

        public enum MType
        {
            Info,
            Warning,
        }
    }
}