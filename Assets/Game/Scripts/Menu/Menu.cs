using UnityEngine;
using static Bloodymary.Game.MenuManager;

namespace Bloodymary.Game
{
    public abstract class Menu : MonoBehaviour
    {
        public Menu _menu { get; private set; }
        public MenuType currentMenu;

        public virtual void Start()
        {
        }

 
    }
}

