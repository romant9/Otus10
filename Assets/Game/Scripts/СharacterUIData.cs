using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodymary.Game
{
    public class СharacterUIData : MonoBehaviour
    {
        public CharacterController Player;
        public string PlayerName { get; set; }
        public Slider HealthSlider;
        public Slider ThrowSlider;
        public TextMeshProUGUI TMPName;
        public TextMeshProUGUI ammunitionCount;
        public TextMeshProUGUI grenadeCount;

        
    }
}

