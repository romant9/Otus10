using UnityEngine;
using static Bloodymary.Game.ControllerTheme;

namespace Bloodymary.Game
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings GSettings;

        [Header("Control Theme Variants")]
        public ControllerTheme LeftPlayerTheme;
        public ControllerTheme RightPlayerTheme;

        [Space(10)]
        public ControllerThemeType SinglePlayerControlTheme;
        public CharacterType MyCharacterIs;
        public GamePlayType gamePlayType;

        //имена персонажей
        [Space(10)]
        public string player1Name;
        public string player2Name;

        [Space(10)]
        [Header("Настройки сложности")]
        public float timeWaitHit = .5f; //пауза перед ударом
        [Range(1, 3)]
        public int _difficulty; //показатели ИИ основаны на показателях игрока и Сложности
        public int spawnCount = 1; //число врагов в первой волне
        public int spawnInterval = 1; //интервал появления врагов

        [Space(10)]

        //стартовые значения
        [Header("Базовые Показатели (Игрок)")]
        public int bulletCount = 10;
        public int grenadeCount = 3;
        public float health = 20;
        public float damageMult = 1;

        [Header("Базовое Оружие (префабы)")]
        public Weapon firstWeapon;
        public Weapon secondayWeapon;

        [Header("Базовые Персонажи (префабы)")]
        public PolicemanController _PolicemanPrefab;
        public HooliganController _HooliganPrefab;

        [Space(10)]
        [Range(1, 5)]
        public int _graphicQuality;


        public enum CharacterType //выбор персонажа, которым управляет игрок. Остальной будет AI
        {
            _Policeman, //Player1
            _Hooligan //Player2
        }

        public enum WinConditionType
        {
            KillEmAll = 0, //убить всех
            SurviveForATime = 1, //выжить в течение заданного времени
            SurviveForWaves = 2 //выжить определенное число волн
        }

        public enum GamePlayType
        {
            MainMenu = -1,
            Test = 0,
            PvP = 1, //Игрок против игрока
            Duel = 2, //Игрок против ИИ
            Wave = 3, //Игрок против толпы разной численности
            Boss = 4 //Игрок против Босса
        }

        public enum DifficultyType
        {
            Easy = 0,
            Normal = 1,
            Hard = 2
        }

        private void Awake()
        {
            GSettings = this;
        }

        public void Start()
        {               
        }

        public ControllerTheme GetControlTheme()
        {
            return SinglePlayerControlTheme == ControllerThemeType.LeftPlayerTheme ? LeftPlayerTheme : RightPlayerTheme;
        }

    }

    public static class GameSettingsGlobals
    {
        public static ControllerTheme LeftPlayerTheme;
        public static ControllerTheme RightPlayerTheme;

        public static int bulletCount = 10;
        public static int grenadeCount = 3;
        public static float health = 20;
        public static float damageMult = 1;
    }
}

