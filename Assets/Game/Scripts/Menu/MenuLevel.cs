using System;
using TMPro;

namespace Bloodymary.Game
{
    public class MenuLevel : Menu
    {
        GameManager GManager => GameManager.GManager;

        public TMP_Text victoryStatus;
        public TMP_Text levelName;
        public TMP_Text gameTime;
        public TMP_Text killed;

        private void OnEnable()
        {
            SetStats();
        }

        private void SetStats()
        {
            victoryStatus.text = GManager.victoryStats;
            levelName.text = GManager.Stats.levelName;

            gameTime.text = ConvertTime(GManager.Stats.gameTime);
            killed.text = GManager.killed.ToString();

        }

        private string ConvertTime(float t)
        {
            TimeSpan time = TimeSpan.FromSeconds(t);
            string displayTime = time.ToString("mm':'ss");
            return displayTime;
        }
    }
}

