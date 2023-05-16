using System.Collections;
using UnityEngine;

namespace Bloodymary.Game
{
    public class PlayStep : PlaySound
    {
        private CharacterController character;
        private AiController enemy;

        private GameManager GManager => GameManager.GManager;
        bool isWalk;
        private bool isPlayable;

        public void Awake()
        {
            SetAudioSource(GetComponent<AudioSource>());
            character = GetComponentInParent<CharacterController>();
            enemy = GetComponentInParent<AiController>();
        }
        private void Start()
        {
            isPlayable = true;
        }
        private void OnEnable()
        {
            StartCoroutine(Initialize());
        }
        private void OnDisable()
        {
            GManager._SettingsSound.OnEnemyStepPlayableChange -= SetPlayable;
        }
        IEnumerator Initialize()
        {
            yield return new WaitForEndOfFrame();
            GManager._SettingsSound.OnEnemyStepPlayableChange += SetPlayable;
        }

        void Update()
        {
            if (!isPlayable) return;
            isWalk = character.isAI ? enemy.isActive : character._acceleration > 0;
        }

        public void SetPlayable()
        {
            if (character.isAI)
                isPlayable = GManager._SettingsSound.IsEnemyStepPlayable;
        }

        public void OnTriggerEnter(Collider ground)
        {
            if (isWalk && isPlayable)
            {
                if (ground.gameObject.layer == 6)
                {
                    PlaySoundEffect("Step");
                }
            }
        }
    }
}