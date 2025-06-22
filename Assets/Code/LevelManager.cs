using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace JamSpace
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private CopyView copyView;
        [SerializeField]
        private LevelSetting levelSetting;

        public static int currLevel
        {
            get => PlayerPrefs.GetInt("curr_level", 1);
            set => PlayerPrefs.SetInt("curr_level", value);
        }

        public static LevelData currLevelData
        {
            get
            {
                var json = PlayerPrefs.GetString("curr_level_data", null);
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LevelData>(json);
            }
            set => PlayerPrefs.SetString("curr_level_data", value.ToJson());
        }

        public static LevelData currLevelPlayerData
        {
            get
            {
                var json = PlayerPrefs.GetString("curr_level_player_data", null);
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LevelData>(json);
            }
            set => PlayerPrefs.SetString("curr_level_player_data", value.ToJson());
        }

        private void Awake() { Application.targetFrameRate = 60; }

        public void Start()
        {
            UniTask.NextFrame().ContinueWith(() =>
            {
                var data = currLevelData;
                var playerData = currLevelPlayerData;
                if (data is null)
                {
                    var rand = new System.Random(currLevel);
                    data = levelSetting.Get(currLevel, rand);
                    playerData = null;
                }

                if (playerData is null)
                {
                    playerData = new(data);
                    playerData.SetAllEmpty();
                }

                copyView.Show(data, playerData);

                data.OnAnyChange += () => currLevelData = data;
                playerData.OnAnyChange += () => currLevelPlayerData = playerData;
            }).Forget();
        }
    }
}