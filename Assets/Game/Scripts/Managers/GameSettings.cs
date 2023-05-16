using UnityEngine;
using UnityEngine.Audio;
using static Bloodymary.Game.ControllerTheme;

namespace Bloodymary.Game
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings GSettings { get; private set; }

        [Header("Control Theme Variants")]
        public ControllerTheme LeftPlayerTheme;
        public ControllerTheme RightPlayerTheme;

        [Space(10)]
        public ControllerThemeType SinglePlayerControlTheme;
        public CharacterType MyCharacterIs;
        public GamePlayType gamePlayType;

        //����� ����������
        [Space(10)]
        public string player1Name;
        public string player2Name;

        [Space(10)]
        [Header("��������� ���������")]
        public float timeWaitHit = .5f; //����� ����� ������
        //[Range(1, 3)]
        //public int _difficulty; 
        public DifficultyType difficultyType = DifficultyType.Custom; //���������� �� �������� �� ����������� ������ � ���������
        public int spawnCount = 1; //����� ������ � ������ �����
        public int spawnInterval = 1; //�������� ��������� ������

        [Space(10)]

        //��������� ��������
        [Header("������� ���������� (�����)")]
        public int bulletCount = 10;
        public int grenadeCount = 3;
        public float health = 20;
        public float damageMult = 1;

        [Header("������� ������ (�������)")]
        public Weapon firstWeapon;
        public Weapon secondayWeapon;

        [Header("������� ��������� (�������)")]
        public PolicemanController _PolicemanPrefab;
        public HooliganController _HooliganPrefab;

        [Space(10)]
        [Range(1, 5)]
        public int _graphicQuality;
        public AudioMixer audioMixer;


        public enum CharacterType //����� ���������, ������� ��������� �����. ��������� ����� AI
        {
            _Policeman, //Player1
            _Hooligan //Player2
        }

        public enum WinConditionType
        {
            KillEmAll = 0, //����� ����
            SurviveForATime = 1, //������ � ������� ��������� �������
            SurviveForWaves = 2 //������ ������������ ����� ����
        }

        public enum GamePlayType
        {
            MainMenu = -1,
            Test = 0,
            PvP = 1, //����� ������ ������
            Duel = 2, //����� ������ ��
            Wave = 3, //����� ������ ����� ������ �����������
            Boss = 4 //����� ������ �����
        }

        public enum DifficultyType
        {
            Easy = 0,
            Normal = 1,
            Hard = 2,
            Custom = 3
        }

        private void Awake()
        {
            if (GSettings != null)
            {
                Destroy(gameObject);
                return;
            }

            GSettings = this;
            DontDestroyOnLoad(gameObject);
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

