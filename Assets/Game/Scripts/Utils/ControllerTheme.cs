using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodymary.Game
{
    [Serializable]
    public class ControllerTheme
    {
        public KeyCode _turboKey;
        public KeyCode _moveKey;
        public KeyCode _rotateLeftKey;
        public KeyCode _rotateRightKey;
        public KeyCode _jumpKey;
        public KeyCode _attackKey;
        public KeyCode _switchWeapon;
        public KeyCode _throwRange;
        public KeyCode _healKey;

        public enum ControllerThemeType
        {
            LeftPlayerTheme = 1,
            RightPlayerTheme = 2
        }
    }
}

