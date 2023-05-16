using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

namespace Bloodymary.Game
{
    public class SettingsSound : MonoBehaviour
    {
        [SerializeField] private List<SoundUIData> soundUIGroup = new List<SoundUIData>();
        private AudioMixer audioMixer;
        public Toggle enemyStepPlayableChange;
        public Action OnEnemyStepPlayableChange;
        public bool IsEnemyStepPlayable { get; set; }

        void Start()
        {
            foreach (var sound in soundUIGroup)
            {
                audioMixer.GetFloat(sound.groupName, out float value);
                sound.slider.value = value;
                sound.textValue.text = (value + 40).ToString();
            }
            
        }

        private void OnEnable()
        {
            audioMixer = GameSettings.GSettings.audioMixer;

            foreach (var sound in soundUIGroup)
            {
                sound.audioMixer = audioMixer;
                sound.slider.onValueChanged.AddListener(sound.SliderValueChanged);
            }
            IsEnemyStepPlayable = enemyStepPlayableChange.isOn;
            enemyStepPlayableChange.onValueChanged.AddListener(SwitchEnemyStep);
        }

        private void OnDisable()
        {
            foreach (var sound in soundUIGroup)
            {
                sound.slider.onValueChanged.RemoveAllListeners();
            }
            enemyStepPlayableChange.onValueChanged.RemoveAllListeners();
        }

        private void SwitchEnemyStep(bool condition)
        {
            IsEnemyStepPlayable = condition;
            OnEnemyStepPlayableChange?.Invoke();
        }

        [Serializable]
        private class SoundUIData
        {
            public string groupName;
            public TextMeshProUGUI textValue;
            public Slider slider;
            public AudioMixer audioMixer { get; set; }

            public void SliderValueChanged(float value)
            {
                float relValue;
                relValue = (value <= -40) ? -80 : value;
                audioMixer.SetFloat(groupName, relValue);

                textValue.text = (value + 40).ToString();
            }
        }
    }
}