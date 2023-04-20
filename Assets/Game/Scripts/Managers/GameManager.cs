using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Bloodymary.Game.GameSettings;
using static Bloodymary.Game.MenuManager;

namespace Bloodymary.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager GManager;

        public �haracterUIData _PolicemanUI; //���������� UI Screen Space, ������������ ��� ������ ������
        public �haracterUIData _HooliganUI;
        public Transform CharactersGroup;
        public List<CharacterController> Characters { get; private set; }
        public CharacterType MyCharacterIs;

        [Space(10)]
        public bool manualActivate; //������������� �������� �������� ����� �������� ��� ������ ���� ������� 

        public bool AIOn; //�������� AI (����-���������) ��� PvP
        public bool AIStatic; //AI (����-���������) ���������, ������������ � ������ trainingMode
        public bool trainingMode; //����� ����������: player �� �������� �����, � ����� ��������, �� ����������������� ��� ���������� ������
        public Transform Player { get; private set; }       
        public SpawnController Spawn { get; private set; }
        public InfoLog GameLog { get; private set; }
        public GameStats Stats { get; private set; }
        public string victoryStats { get; private set; }
        private MenuManager MManager => MenuManager.MManager;
        public bool checkVictory { get; set; }
        public int killed { get; private set; }

        private void Awake()
        {
            GManager = this;
            Characters = new List<CharacterController>();

            //��������� �������. ��� ������� ���� �� ���������
#region debug case
            //if (GSettings == null)
            //{
            //    var settings = Resources.Load("GameSettings") as GameObject;
            //    GSettings = Instantiate(settings).GetComponent<GameSettings>();
            //}
#endregion
        }
        private void Start()
        {
            checkVictory = true;

            Spawn = GetComponent<SpawnController>();
            GameLog = GetComponent<InfoLog>();
            Stats = GetComponent<GameStats>();

            GetPlayerNames();
            PreInit();

            if (manualActivate) 
            {
                string content = "������� ������� ������, ����� ������...";
                GameLog.ShowInfo(manualActivate, content);
                StartCoroutine(WaitForPreInit());
            }           
        }

        private void GetPlayerNames()
        {
            MyCharacterIs = GSettings.MyCharacterIs;
            string policeman = "�����������";
            string hooligan = "�������";

            Debug.Log("name: " + GSettings.player1Name);

            if (MyCharacterIs == CharacterType._Policeman) 
            {
                _PolicemanUI.PlayerName = !string.IsNullOrEmpty(GSettings.player1Name) ? GSettings.player1Name : policeman;
                _HooliganUI.PlayerName = GSettings.gamePlayType == GamePlayType.PvP || GSettings.gamePlayType == GamePlayType.Test 
                    ? !string.IsNullOrEmpty(GSettings.player2Name) ? GSettings.player2Name : hooligan : hooligan;
            }
            else
            {
                _PolicemanUI.PlayerName = GSettings.gamePlayType == GamePlayType.PvP || GSettings.gamePlayType == GamePlayType.Test 
                    ? !string.IsNullOrEmpty(GSettings.player2Name) ? GSettings.player2Name : policeman : policeman;
                _HooliganUI.PlayerName = !string.IsNullOrEmpty(GSettings.player1Name) ? GSettings.player1Name : hooligan;
            }

        }
        private IEnumerator WaitForPreInit()
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            string content = "���� ���� ������";
            GameLog.ShowInfo(false, content);
            Debug.Log("���� ���� ������");

            //���� �������
            if (Stats.levelName == "Wave")            
                Spawn.Initialize(1); 
            else
                AIOn = true;
            PreInit();
        }

        public void PreInit()
        {
            foreach (var character in Characters)
            {
                character.CharacterPreInit();
            }
        }

        //GUI - ToggleAI
        public void AIOnSwitch(Toggle tg)
        {
            Text text = tg.transform.Find("Label").GetComponent<Text>();
            AIOn = tg.isOn;
            text.text = AIOn ? "AI ���������" : "AI ��������";

            PreInit();
        }

        //GUI - ToggleAI (go = TogglePauseAI)
        public void SetActiveGO(GameObject go)
        {
            go.SetActive(!go.activeSelf);
            go.GetComponent<Toggle>().isOn = false;
        }

        //GUI - ToggleAI
        public void AIPauseToggle(Toggle tg)
        {
            Text text = tg.transform.Find("Label").GetComponent<Text>();
            text.text = tg.isOn ? "�� ����������" : "�� ���������";

            AIPause(!tg.isOn);
        }
        void AIPause(bool stop)
        {
            foreach (var character in EnemyList())
            {
                var AI = character.GetComponent<AiController>();
                AI.Execute(stop);
            }
        }

        public void SpawnControl(Toggle tg)
        {
            Text text = tg.transform.Find("Label").GetComponent<Text>();
            text.text = tg.isOn ? "������������ �����" : "��������� �����";

            if (tg.isOn) Spawn.Initialize(1);
            else Spawn.ResetSpawn();

            Spawn.isActive = tg.isOn;
        }

        public void SpawnCount(Slider tg)
        {
            Text text = tg.transform.parent.Find("Value").GetComponent<Text>();
            text.text = tg.value.ToString();
            Spawn.spawnCount = (int)tg.value;

        }
        public void SpawnInterval(Slider tg)
        {
            Text text = tg.transform.parent.Find("Value").GetComponent<Text>();
            text.text = tg.value.ToString();
            Spawn.timeInterval = (int)tg.value;

        }

        //GUI
        public void CharacterSwitch(TMP_Dropdown dp)
        {
            MyCharacterIs = (CharacterType)dp.value;
            GSettings.MyCharacterIs = MyCharacterIs;
        }

        public void SetPlayer(Transform player)
        {
            Player = player;
        }

        
        public void SetPlayerUIData(�haracterUIData data)
        {
            //data.PlayerName = MyCharacterIs == CharacterType._Policeman ? GSettings.player1Name : GSettings.player2Name;
            data.TMPName.text = data.PlayerName;
            data.ammunitionCount.text = GSettings.bulletCount.ToString();
            data.grenadeCount.text = GSettings.grenadeCount.ToString();
        }

        public List<CharacterController> EnemyList()
        {
            var list = new List<CharacterController>();
            list.AddRange(Characters);
            if (Player) list.Remove(GManager.Player.GetComponent<CharacterController>());
            else list.Remove(Characters.First());
            
            return list;
        }

        public Transform GetNearestEnemy(Transform player = null)
        {
            if (!player) player = Player;
            var list = EnemyList();
            if (list.Count == 1) return Characters.FirstOrDefault().transform;
            else
            {
                Transform nearestEnemy = null;

                int nearestIndex = 0;
                float distanceCurrent;
                float distanceNext;

                for (int i = 1; i < list.Count; i++)
                {
                    nearestEnemy = list[nearestIndex].transform;
                    distanceCurrent = Vector3.Distance(list[nearestIndex].transform.position, player.position);
                    distanceNext = Vector3.Distance(list[i].transform.position, player.position);

                    if (distanceNext < distanceCurrent)
                    {
                        nearestEnemy = list[i].transform;
                        nearestIndex = i;
                    }
                }
                return nearestEnemy;
            } 
        }


        public void CheckVictory()
        {
            if (checkVictory)
            {
                if (AIOn)
                {
                    if (Player && Characters.Count < 2)
                    {
                        victoryStats = "�� ��������!";
                        checkVictory = false;
                        AIOn = false;
                    }

                    if (!Player)
                    {
                        victoryStats = "�� ���������!";
                        checkVictory = false;
                        AIOn = false;
                    }

                    killed = Spawn.spawnCount - Characters.Count + 1;
                }
                else
                {
                    if (Characters.Count < 2)
                    {
                        victoryStats = "������� �����  " + Characters.First().CurrentUIData.PlayerName;
                        checkVictory = false;
                    }

                    killed = 1;
                }

                if (!checkVictory)
                    MManager.SetCurrentMenu(MenuManager.MenuType.Level);
            }
        }

        private void Update()
        {
            if (AIOn)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    AIPause(MManager.currentMenu != MenuType.Level);
                }
            }
        }

        //public �haracterUIData GetPlayerUIData(CharacterController player, bool AIOn)
        //{
        //    return player._characterType == (!AIOn ? CharacterType._Policeman : MyCharacterIs) ? _PolicemanUI : _HooliganUI;
        //}
    }


}

