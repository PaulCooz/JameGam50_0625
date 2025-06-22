using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class TutorView : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup group;
        [SerializeField]
        private InputAction clickAction;

        [SerializeField]
        private MessageView messageView;
        [SerializeField]
        private Button tutorButton;

        [SerializeField]
        private List<RectTutor> tutors;

        [SerializeField]
        private UnityEvent clickForInfo;

        private MessageView _infoMessage;
        private readonly List<Component> _toRemove = new();
        private Camera _camera;
        private bool _inProcess;

        private Component _lastTouchedComp;

        public bool open => group.blocksRaycasts;

        private void Awake()
        {
            _camera = Camera.main;
            clickAction.Enable();
        }

        public void Show()
        {
            if (_inProcess)
                return;
            _inProcess = true;
            _infoMessage = Instantiate(messageView, group.transform, true);
            _toRemove.Add(_infoMessage);

            var tutorButtonDup = Instantiate(tutorButton, group.transform, true);
            tutorButtonDup.onClick.AddListener(Hide);
            _toRemove.Add(tutorButtonDup);

            group.blocksRaycasts = true;
            group.DOFade(1, 0.3f).OnComplete(() =>
            {
                _infoMessage.PushLocal("Click anywhere to see information");
                _inProcess = false;
            });
        }

        private void Hide()
        {
            _inProcess = true;
            group.blocksRaycasts = false;
            group.DOFade(0, 0.3f).OnComplete(() =>
            {
                DestroyLast();

                this.DestroyAll(_toRemove);
                _inProcess = false;
            });
        }

        private void DestroyLast()
        {
            if (_lastTouchedComp is not null)
            {
                Destroy(_lastTouchedComp.gameObject);
                _lastTouchedComp = null;
            }
        }

        private void Update()
        {
            if (group.blocksRaycasts && clickAction.WasPerformedThisFrame())
            {
                clickForInfo.Invoke();

                var pos = Pointer.current.position.value;
                foreach (var t in tutors)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(t.rect, pos, _camera))
                    {
                        DestroyLast();

                        _lastTouchedComp = Instantiate(t.rect, group.transform, true);
                        _infoMessage.PushLocal(t.message);
                        break;
                    }
                }
            }
        }

        [Serializable]
        public struct RectTutor
        {
            public RectTransform rect;
            public string message;
        }
    }
}