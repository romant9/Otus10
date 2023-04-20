using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Bloodymary.Game.GameSettings;

namespace Bloodymary.Game
{
    public class MenuMain : Menu
    {
        public GameObject GOPlayer2;
        public GameObject GOSpawnCount;
        public TextMeshProUGUI textSpawnCount;

        public override void Start()
        {
            ActivateGOPlayer2();
            ActivateGOSpawnCount();
        }
        public void SetPlaye1Name(TMP_InputField field)
        {
            GSettings.player1Name = field.text;
        }
        public void SetPlaye2Name(TMP_InputField field)
        {
            GSettings.player2Name = field.text;
        }

        public void ActivateGOPlayer2()
        {
            var cond = GSettings.gamePlayType == GamePlayType.PvP || GSettings.gamePlayType == GamePlayType.Test;
            GOPlayer2.SetActive(cond);
        }

        public void ActivateGOSpawnCount()
        {
            var cond = GSettings.gamePlayType == GamePlayType.Wave || GSettings.gamePlayType == GamePlayType.Test;
            GOSpawnCount.SetActive(cond);
        }

        public void SetSpawnCount(Slider sl)
        {
            textSpawnCount.text = sl.value.ToString();
            GSettings.spawnCount = (int)sl.value;
        }

    }
}

