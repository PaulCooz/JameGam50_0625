using System;
using DG.Tweening;
using UnityEngine;

namespace JamSpace
{
    public sealed class WhaleView : MonoBehaviour
    {
        [SerializeField]
        private Vector2 minMaxY;
        [SerializeField]
        private float speed = 1;

        private float _time = 0.5f;

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