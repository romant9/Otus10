using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodymary.Game
{
    public class MenuSettings : Menu
    {
        [SerializeField] private List<SubMenuGroup> SettingSubMenu = new List<SubMenuGroup>();

        public override void Start()
        {
            if (SettingSubMenu.Count > 1)
            {
                foreach (var menu in SettingSubMenu)
                {
                    menu.bt.onClick.AddListener(() => OnClick(menu.canvas));
                }
            }

        }

        void OnClick(CanvasGroup canvas)
        {
            foreach (var menu in SettingSubMenu)
            {
                menu.canvas.alpha = 0;
                menu.canvas.blocksRaycasts = false;
            }
            canvas.alpha = 1;
            canvas.blocksRaycasts = true;
        }

        [Serializable]
        private class SubMenuGroup
        {
            public string name;
            public Button bt;
            public CanvasGroup canvas;

        }
    }
}

