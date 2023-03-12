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
    public bool AIOn; //включить AI (авто-противник)
    public float timeWaitHit = .5f; //пауза перед ударом

    public WeaponInfo _bullet; //префаб пули

    public enum CharacterType //выбор персонажа, которым управляет игрок. Остальной будет AI
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
        text.text = AIOn ? "AI включено" : "AI выключено";
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
