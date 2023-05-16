using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodymary.Game
{
    [Serializable]
    public class EffectRouter: MonoBehaviour
    {
        [SerializeField] private Transform fxEffect;
        [SerializeField] private PlaySound soundEffect;

        public enum EffectType
        {
            Sprite,
            Particles,
            Gibrid,
            None
        }

        public EffectType effectType = EffectType.Sprite;

        public GameObject GetInstanceFxEffect()
        {
            return Instantiate(fxEffect.gameObject, fxEffect.parent);
        }

        private void Start()
        {
           
        }
      
        public void ExecuteParticlesEffect(Vector3 pos, Quaternion rot)
        {
            fxEffect.position = pos;
            fxEffect.rotation = rot;

            foreach (var effect in fxEffect.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
            }
        }
        public void PlaySound(string name)
        {
            soundEffect.PlaySoundEffect(name);
        }
        public AudioSource GeAaudioSource()
        {
            return soundEffect.GetAudioSource();
        }
    }
}

