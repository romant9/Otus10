using Bloodymary.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace Bloodymary.Game
{
    public class SettingsGraphics : MonoBehaviour
    {
        public TMP_Dropdown dpQuality;
        public Toggle tgPostEffects;
        public Toggle tgAO;
        public Slider tgEdgeDetect;

        private PostProcessLayer _PostProcessLayer;

        public void Start()
        {
            _PostProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
        }

        public void SetQuality()
        {
            QualitySettings.SetQualityLevel(QLevel(dpQuality.value));
        }

        public int QLevel(int value)
        {
            switch (value)
            {
                case 0: return 2;
                case 1: return 3;
                case 2: return 5;
                default: return 3;
            }
        }

        public void SwitchPostEffects()
        {
            if (tgPostEffects.isOn)
                _PostProcessLayer.volumeLayer |= 1 << LayerMask.NameToLayer("PostProcessEffects");
            else
                _PostProcessLayer.volumeLayer ^= 1 << LayerMask.NameToLayer("PostProcessEffects");
        }

        public void SwitchAO()
        {
            if (tgAO.isOn)
                _PostProcessLayer.volumeLayer |= 1 << LayerMask.NameToLayer("PostProcessAO");
            else
                _PostProcessLayer.volumeLayer ^= 1 << LayerMask.NameToLayer("PostProcessAO");
        }

        public void SetContur()
        {
            int value = Mathf.RoundToInt(tgEdgeDetect.value);
            var effect = Camera.main.GetComponent<UTS_EdgeDetection>();

            switch (value)
            {
                case 0:
                    effect.enabled = false;
                    break;
                case 1:
                    effect.enabled = true;
                    effect.filterPower = .25f;
                    effect.threshold = .3f;
                    break;
                case 2:
                    effect.enabled = true;
                    effect.filterPower = .4f;
                    effect.threshold = .4f;
                    break;
                case 3:
                    effect.enabled = true;
                    effect.filterPower = .7f;
                    effect.threshold = .5f;
                    break;
            }
        }
    }
}