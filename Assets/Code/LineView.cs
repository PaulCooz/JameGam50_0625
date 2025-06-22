using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace JamSpace
{
    public sealed class LineView : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private Vector2 minMaxSpeed;

        private readonly int _colorId = Shader.PropertyToID("_Color");
        private readonly int _lineColorId = Shader.PropertyToID("_LineColor");
        private readonly int _widthId = Shader.PropertyToID("_LineWidth");
        private readonly int _speedId = Shader.PropertyToID("_Speed");
        private readonly int _offsetId = Shader.PropertyToID("_Offset");

        private void Awake()
        {
            image.material = Instantiate(image.material);
            image.material.SetColor(_colorId, image.color);
            image.material.SetFloat(_speedId, Random.Range(minMaxSpeed.x, minMaxSpeed.y));
            image.material.SetFloat(_offsetId, Random.value);
        }

        public void Setup(RectTransform from, RectTransform to)
        {
            var wToL = transform.parent.worldToLocalMatrix;
            var localF = wToL.MultiplyPoint(from.position);
            var localT = wToL.MultiplyPoint(to.position);

            var rt = (RectTransform)transform;
            rt.localPosition = (localF + localT) / 2f;

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(from.rect.height, to.rect.height) / 3f);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Vector3.Distance(localF, localT));

            var angle = Mathf.Atan2(localT.y - localF.y, localT.x - localF.x) * Mathf.Rad2Deg;
            rt.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}