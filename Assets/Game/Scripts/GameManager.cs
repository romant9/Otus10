using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public PolicemanController _Policeman;
    public HooliganController _Hooligan;
    [HideInInspector]
    public Transform enemyTarget;
    public bool AIOn; //�������� AI (����-���������)
    public float timeWaitHit = .5f; //����� ����� ������

    public WeaponInfo _bullet; //������ ����

    public enum CharacterType //����� ���������, ������� ��������� �����. ��������� ����� AI
    {
        _Policeman,
        _Hooligan
    }

    public CharacterType MyCharacterIs;

    private void Awake()
    {
        GM = this;
    }

    public void AIOnSwitch(Toggle tg)
    {
        Text text = tg.transform.Find("Label").GetComponent<Text>();
        AIOn = tg.isOn;
        text.text = AIOn ? "AI ��������" : "AI ���������";
        _Policeman.CharacterInit();
        _Hooligan.CharacterInit();
    }
    public void CharacterSwitch(Dropdown dp)
    {
        switch (dp.value)
        {
            case 0:
                MyCharacterIs = CharacterType._Policeman;
                break; 
            case 1:
                MyCharacterIs = CharacterType._Hooligan;
                break;

        }
    }
}
