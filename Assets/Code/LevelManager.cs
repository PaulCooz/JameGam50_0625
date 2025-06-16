using UnityEngine;
using UnityEngine.Serialization;

namespace JamSpace
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private CommandView commandPartView;

        public void OpenCommandResolving()
        {
            commandPartView.Show();
        }
    }
}