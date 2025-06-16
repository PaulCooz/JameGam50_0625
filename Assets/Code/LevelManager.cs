using UnityEngine;
using UnityEngine.Serialization;

namespace JamSpace
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private CopyView copyPartView;

        public void OpenCopy()
        {
            copyPartView.Show();
        }
    }
}