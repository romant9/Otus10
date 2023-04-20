using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bloodymary.Game
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager MManager;

        public List<GameObject> menuList; //вносим в редакторе все menu из Canvas

        //public MenuMain menuMain;
        //public MenuSettings menuSettings;
        //public MenuLevel menuLevel;
        //public GameObject UILevel;


        public MenuType currentMenu = MenuType.Main;
        private MenuType lastMenu;

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
        }

        public void SetCurrentMenu(MenuType menu)
        {
            GetMenu();
            var currentMenuGO = menuList.Find(x => x.GetComponent<Menu>().currentMenu == menu);
            currentMenuGO.SetActive(true);
            currentMenu = menu;
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
            SetCurrentMenu(currentMenu != MenuType.None ? lastMenu : MenuType.Level);

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

