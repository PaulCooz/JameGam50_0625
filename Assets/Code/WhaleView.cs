using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public sealed class WhaleView : MonoBehaviour
    {
        [SerializeField]
        private List<Sprite> planets;
        [SerializeField]
        private SpriteRenderer leftPlanet, rightPlanet;

        [SerializeField]
        private Vector2 minMaxY;
        [SerializeField]
        private float speed = 1;

        private float _time = 0.5f;
        private Vector3 _startScale;

        public UniTaskCompletionSource animTcs { get; private set; } = new();

        private void Awake()
        {
            planets = planets.RandomShuffle();
            leftPlanet.sprite = planets.First();
            rightPlanet.sprite = planets.Last();

            _startScale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        private void Start()
        {
            const float duration = 1f;
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .Append(leftPlanet.transform.DOMoveX(-30f, duration).SetEase(Ease.Linear))
                .Join(leftPlanet.transform.DOScale(3f, duration).SetEase(Ease.Linear))
                .Join(rightPlanet.transform.DOMoveX(+30f, duration).SetEase(Ease.Linear))
                .Join(rightPlanet.transform.DOScale(3f, duration).SetEase(Ease.Linear))
                .Join(transform.DOScale(_startScale, duration).SetDelay(duration / 2f))
                .OnComplete(() => animTcs.TrySetResult());
        }

        private void Update()
        {
            var pos = transform.position;
            var t = _time % 2.1f;
            if (0f <= t && t < 1f)
                pos.y = Mathf.Lerp(minMaxY.x, minMaxY.y, t);
            else if (1f <= t && t < 1.05f)
                pos.y = pos.y; // chilling
            else if (1.05f <= t && t < 2.05f)
                pos.y = Mathf.Lerp(minMaxY.y, minMaxY.x, t - 1.05f);
            else if (2.05f <= t && t < 2.10f)
                pos.y = pos.y;

            transform.position = pos;

            _time += speed * Time.deltaTime;
        }
    }
}