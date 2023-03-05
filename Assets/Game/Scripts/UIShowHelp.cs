using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowHelp : MonoBehaviour
{
    public GameObject helpObj;
    public void ShowHelp()
    {
        helpObj.SetActive(!helpObj.activeSelf);
    }
}
