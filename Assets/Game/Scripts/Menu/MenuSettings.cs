using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Bloodymary.Game.GameSettings;

namespace Bloodymary.Game
{
    public class MenuSettings : Menu
    {
        public TextMeshProUGUI textMaxHealth;
        public TextMeshProUGUI textDamageMult;
        public TextMeshProUGUI textSpawnCount;
        public GameObject GOSpawnCount;

        public override void Start()
        {
            ActivateGOSpawnCount();
        }

        public void SetDifficulty(TMP_Dropdown dp)
        {
            GSettings._difficulty = dp.value;
        }

        public void SetMaxHealth(Slider sl)
        {
            GSettings.health = sl.value;
            textMaxHealth.text = sl.value.ToString();
        }

        public void SetDamageMult(Slider sl)
        {
            GSettings.damageMult = sl.value/2;
            textDamageMult.text = (sl.value/2).ToString();
        }

        public void SetSpawnCount(Slider sl)
        {
            GSettings.spawnCount = (int)sl.value;
            textSpawnCount.text = sl.value.ToString();
        }
        public void ActivateGOSpawnCount()
        {
            var cond = SceneManager.GetActiveScene().buildIndex != 0 && (GSettings.gamePlayType == GamePlayType.Wave || GSettings.gamePlayType == GamePlayType.Test);
            GOSpawnCount.SetActive(cond);
        }
    }
}

