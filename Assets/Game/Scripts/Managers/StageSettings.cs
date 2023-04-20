using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Bloodymary.Game.GameSettings;

namespace Bloodymary.Game
{
    //настройки текущего уровн€. ”никальный дл€ каждой сцены
    public class StageSettings : MonoBehaviour
    {
        //public static StageSettings instance;
        public MenuManager menuManager;
        public GameManager GManager => GameManager.GManager;

        public Animator _animator; //fade canvas animator
        private Image _FadeImage;
        
        private GamePlayType gamePlayType = GamePlayType.PvP;
        private string sceneName;
        private bool sceneNameChanged;

     
        private void Start()
        {
            _FadeImage = _animator.GetComponent<Image>();
        }

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
            //sceneName = SceneName(gamePlayType);
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

        public void StartNewGame(int index)
        {
            if (sceneNameChanged)
            {
                sceneName = SceneName(gamePlayType);
            }
            else
            {
                sceneName = SceneNameByID(index);
            }
            StartCoroutine(ILoadScene());
        }

        private IEnumerator ILoadScene()
        {
            _animator.SetTrigger("fade");
            yield return new WaitUntil(() => _FadeImage.color.a == 0);

            sceneNameChanged = false;
            
            SceneManager.LoadScene(sceneName);

            Debug.Log("загружаем сцену: " + sceneName);
        }
    }

}

