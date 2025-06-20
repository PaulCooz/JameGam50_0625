using UnityEngine;
using UnityEngine.InputSystem;

namespace JamSpace
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private CopyView copyView;
        [SerializeField]
        private InputAction openInput;

        private void Start()
        {
            openInput.Enable();
        }

        private void Update()
        {
            if (openInput.WasPerformedThisFrame())
                OpenCopy();
        }

        public void OpenCopy()
        {
            copyView.Show();
        }
    }
}