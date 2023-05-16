using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Bloodymary.Game.GameSettings;

namespace Bloodymary.Game
{
    //настройки текущего уровн€. ”никальный дл€ каждой сцены
    public class StageSettings : MonoBehaviour
    {        
        //private ProgressBar progressBar => ProgressBar._ProgressBar; 
        private GamePlayType gamePlayType = GamePlayType.PvP;
        private string sceneName;
        private bool sceneNameChanged;
        
        //GUI - Dropdown_Player
        public void SetMyCharacter(TMP_Dropdown dp)
        {
            GSettings.MyCharacterIs = (CharacterType)dp.value;
        }

        //GUI - Dropdown_Scene     
        public void SetGameType(TMP_Dropdown dp)
        {
            gamePlayType = (GamePlayType)dp.value;
            GSettings.gamePlayType = gamePlayType;
            sceneNameChanged = true;
        }      

        public string SceneName(GamePlayType type)
        {       
            switch (type)
            {
                case GamePlayType.MainMenu: return "MainMenu"; //0             
                case GamePlayType.Test: return "SceneDevelop"; //1
                case GamePlayType.PvP: return "PvP";           //2
                case GamePlayType.Duel: return "Duel";         //3 
                case GamePlayType.Wave: return "Wave";         //4
                case GamePlayType.Boss: return "Boss";         //5 
                default: return "SceneDevelop";
            }
        }
        public string SceneNameByID(int id)
        {
            switch (id)
            {
                case 0: return "MainMenu";             
                case 1: return "SceneDevelop"; 
                case 2: return "PvP";           
                case 3: return "Duel";          
                case 4: return "Wave";         
                case 5: return "Boss";          
                default: return "SceneDevelop";
            }
        }

        //GUI - StartButton
        public void StartNewGame()
        {
            sceneName = SceneName(gamePlayType);
            ProgressBar._ProgressBar.FadeAndLoad(sceneName);
        }
        public void RestartGame()
        {
            sceneName = SceneManager.GetActiveScene().name;
            Debug.Log(sceneName);
            ProgressBar._ProgressBar.FadeAndLoad(sceneName);
        }
        public void BackToMainMenu()
        {
            sceneName = SceneName(GamePlayType.MainMenu);
            ProgressBar._ProgressBar.FadeAndLoad(sceneName);
        }
 
    }

}

