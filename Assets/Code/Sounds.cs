using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace JamSpace
{
    public sealed class Sounds : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private Image soundIcon;
        [SerializeField]
        private Sprite onSoundIcon, offSoundIcon;

        [SerializeField]
        private AudioMixer audioMixer;

        [SerializeField]
        private AudioSource background;
        [SerializeField]
        private List<AudioClip> clips;

        private static bool HasSound
        {
            get => PlayerPrefs.GetInt("sound", 1) != 0;
            set => PlayerPrefs.SetInt("sound", value ? 1 : 0);
        }

        private void Awake()
        {
            RefreshToggle();
            toggle.onValueChanged.AddListener((value) =>
            {
                HasSound = value;
                RefreshToggle();
            });

            return;

            void RefreshToggle()
            {
                toggle.SetIsOnWithoutNotify(HasSound);
                soundIcon.sprite = toggle.isOn ? onSoundIcon : offSoundIcon;

                audioMixer.SetFloat("MasterVolume", toggle.isOn ? 0f : -80f);
            }
        }


        private void OnEnable()
        {
            clips = clips.RandomShuffle();

            PlayBackgroundAsync().Forget();
        }

        private async UniTask PlayBackgroundAsync()
        {
            var index = 0;
            while (isActiveAndEnabled)
            {
                await UniTask.NextFrame();

                background.clip = clips[index];
                background.Play();

                await UniTask.WaitForSeconds(background.clip.length);

                index = (index + 1) % clips.Count;
            }
        }
    }
}