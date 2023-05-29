using System;
using UnityEngine;

namespace Bloodymary.Game
{
    public abstract class ButtonActionSystem : MonoBehaviour
    {
        public IntersectObject intersectComp { get; private set; } //кнопка

        public Action<bool> ActionBase;

        protected virtual void Start()
        {
            intersectComp = GetComponent<IntersectObject>();
        }

        void Update()
        {
        }

        protected void OnEnable() { ActionBase += ActionMethod; }
        protected void OnDisable() { ActionBase -= ActionMethod; }

        protected virtual void ActionMethod(bool condition)
        {
        }

    }
}

