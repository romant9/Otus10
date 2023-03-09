using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public PolicemanController _Policeman;
    public HooliganController _Hooligan;

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
}
