using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodymary.Game
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager MManager;

        public List<GameObject> menuList; //вносим в редакторе все menu из Canvas

        private GameManager GManager;
        private PlaySound SceneMusic;
        private string currentMusic;

        public MenuType currentMenu = MenuType.Main;
        private MenuType lastMenu;

        PostProcessManager post;

        public enum MenuType
        {
            None,
            Main,
            Settings,
            Level
        }

        private void Awake()
        {
            MManager = this;
        }
        void Start()
        {
            if (SceneManager.GetActiveScene().name != "MainMenu") post = Camera.main.GetComponent<PostProcessManager>();
            lastMenu = MenuType.Level;

            try
            {
                GManager = GetComponent<GameManager>();                
            }
            catch
            {
                GManager = null;
            }

            if (GManager) SceneMusic = GManager.SceneMusic;
        }

        public void SetCurrentMenu(MenuType menu)
        {
            GetMenu();
            var currentMenuGO = menuList.Find(x => x.GetComponent<Menu>().currentMenu == menu);
            if (currentMenuGO) currentMenuGO.SetActive(true);
            currentMenu = menu;

            if (post) post.ChangeFocus(currentMenu != MenuType.None);

            if (SceneMusic && GManager.checkVictory)
            {
                PlayMusicTheme();
            }

        }
        void PlayMusicTheme()
        {
            bool condition = currentMenu != MenuType.None;
            string musicTheme = condition ? "Menu" : currentMusic;
            SceneMusic.PlaySoundEffect(musicTheme);
        }

        public GameObject GetCurrentMenuGO(MenuType menu)
        {
            return menuList.Find(x => x.GetComponent<Menu>().currentMenu == menu);
        }

        public void SetCurrentMenuGO(GameObject menu)
        {
            GetMenu();
            var currentMenuGO = menuList.Find(x => x == menu);
            currentMenuGO.SetActive(true);
            currentMenu = menu.GetComponent<Menu>().currentMenu;


        }

        private void GetMenu()
        {
            lastMenu = currentMenu;
            foreach (GameObject go in menuList)
            {
                go.SetActive(false);
            }
        }

        public void SetBack()
        {
            if (SceneMusic)
            {
                if (currentMenu == MenuType.None)
                    currentMusic = SceneMusic.GetAudioClipName();
            } 

            SetCurrentMenu(currentMenu == MenuType.None ? lastMenu : MenuType.None);
            //SetCurrentMenu(MenuType.None);
            if (post) post.ChangeFocus(currentMenu != MenuType.None);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetBack();
            }
        }
    }
}

