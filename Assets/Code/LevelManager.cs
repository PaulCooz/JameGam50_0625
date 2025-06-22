using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private CopyView copyView;
        [SerializeField]
        private LevelSetting[] levelSettings;
        [SerializeField]
        private TMP_Text levelTMP;
        [SerializeField]
        private GraphicRaycaster raycaster;

        public static int currLevel
        {
            get => PlayerPrefs.GetInt("curr_level", 1);
            set => PlayerPrefs.SetInt("curr_level", value);
        }

        public static int currLevelSeed
        {
            get => PlayerPrefs.GetInt("curr_level_seed", 1);
            set => PlayerPrefs.SetInt("curr_level_seed", value);
        }

        public static LevelData currLevelData
        {
            get
            {
                var json = PlayerPrefs.GetString("curr_level_data", null);
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LevelData>(json);
            }
            set => PlayerPrefs.SetString("curr_level_data", value?.ToJson());
        }

        public static LevelData currLevelPlayerData
        {
            get
            {
                var json = PlayerPrefs.GetString("curr_level_player_data", null);
                return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<LevelData>(json);
            }
            set => PlayerPrefs.SetString("curr_level_player_data", value?.ToJson());
        }

        private LevelData _data, _playerData;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            _screenHeight = Screen.height;
            _screenWidth = Screen.width;
        }

        public void Start()
        {
            UniTask.NextFrame().ContinueWith(() =>
            {
                _data = currLevelData;
                _playerData = currLevelPlayerData;
                if (_data is null)
                {
                    var rand = new System.Random(currLevelSeed);
                    var settings = currLevel - 1 >= levelSettings.Length
                        ? levelSettings.Last()
                        : levelSettings[currLevel - 1];

                    _data = settings.Get(currLevel, rand);
                    _playerData = null;
                }

                if (_playerData is null)
                {
                    _playerData = new(_data);
                    _playerData.SetAllEmpty();
                }

                levelTMP.text = $"LEVEL {_data.number}";
                copyView.Show(_data, _playerData);

                _data.OnAnyChange += OnDataChange;
                _playerData.OnAnyChange += OnPlayerDataChange;
            }).Forget();
        }

        private int _screenHeight, _screenWidth;

        private void Update()
        {
            if (Time.frameCount % 30 == 0 && raycaster.enabled &&
                (_screenHeight != Screen.height || _screenWidth != Screen.width))
            {
                _screenHeight = Screen.height;
                _screenWidth = Screen.width;

                ReloadLevelForceAsync().Forget();
            }
        }

        public void Check()
        {
            if (copyView.IsAllDone())
                NextLevelAsync(true).Forget();
            else
                MessageView.Push("Wrong copy  ;(");
        }

        private async UniTask ReloadLevelForceAsync()
        {
            raycaster.enabled = false;

            _data.OnAnyChange -= OnDataChange;
            _playerData.OnAnyChange -= OnPlayerDataChange;

            await copyView.Hide();

            Start();
            raycaster.enabled = true;
        }

        private async UniTask NextLevelAsync(bool isWin)
        {
            raycaster.enabled = false;

            MessageView.Push(isWin ? "ALL DONE!" : "ENERGY IS OVER  ¯\\_(- ъ -)_/¯");

            _data.OnAnyChange -= OnDataChange;
            _playerData.OnAnyChange -= OnPlayerDataChange;
            _data = _playerData = null;
            currLevelData = currLevelPlayerData = null;

            if (isWin)
                currLevel++;
            currLevelSeed++;

            copyView.ForceShowLeft();

            await UniTask.WaitForSeconds(4.5f);
            MessageView.Push("");
            await UniTask.WaitForSeconds(0.5f);

            await copyView.Hide();

            Start();
            raycaster.enabled = true;
        }

        private void OnDataChange()
        {
            currLevelData = _data;
            if (_data.currentEnergy <= 0)
                NextLevelAsync(false).Forget();
        }

        private void OnPlayerDataChange() => currLevelPlayerData = _playerData;
    }
}