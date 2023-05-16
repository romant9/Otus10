using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using static Bloodymary.Game.WeaponManager;
using static Bloodymary.Game.GameManager;

namespace Bloodymary.Game
{
    [Serializable]
    public class Weapon : MonoBehaviour
    {
        public CharacterController WeaponOwner { get; protected set; }
        public WeaponType _weaponType;

        public float _weaponDamage;
        public string _weaponName;
        [HideInInspector]
        public bool _isDamaging;

        //ToDo : Создать дочерние классы для 3-х типов оружия
        [ShowIf("_weaponType", WeaponType.Fire)]
        public float _weaponSpeed;

        private Bullet _wBulletOrigin;
        private Bullet _wBulletCurrent;

        private EffectRouter Effects;
        public AudioSource _audioSource { get; private set; }

        [ShowIf("_weaponType", WeaponType.Throw)]
        public float _damageRadius, _detonationTime;

        bool grenadeFallDown;

        protected virtual void Start()
        {
            WeaponOwner = GetComponentInParent<CharacterController>();
            if (_weaponType == WeaponType.Fire) 
            {
                _wBulletOrigin = transform.GetComponentInChildren<Bullet>(true);
            }
            Effects = transform.GetComponent<EffectRouter>();
            _audioSource = transform.GetComponent<AudioSource>();
        }

        public void InitShoot()
        {
            if (!_wBulletOrigin) return;

            Effects.PlaySound("Shoot");

            _wBulletCurrent = Instantiate(_wBulletOrigin.gameObject, _wBulletOrigin.transform.parent).GetComponent<Bullet>();
            _wBulletCurrent.SetOwner(this);
            _wBulletCurrent.gameObject.SetActive(true);           
        }       

        public void InitThrow(float mult)
        {
            if (!GetComponent<Rigidbody>()) return;

            StartCoroutine(GrenadeFly(GetComponent<Rigidbody>(), mult));
        }
        private IEnumerator GrenadeFly(Rigidbody rigid, float mult)
        {            
            rigid.isKinematic = false;
            rigid.transform.parent = null;
            rigid.AddForce((WeaponOwner.transform.forward + Vector3.up * .3f) * 300 * mult * rigid.mass);
            yield return new WaitUntil(() => Vector3.Distance(WeaponOwner.transform.position, transform.position) > 2);
            rigid.GetComponent<Collider>().isTrigger = false;
            yield return new WaitUntil(() => grenadeFallDown);
            
            yield return new WaitForSeconds(_detonationTime);
            UI_Effects(WeaponType.Throw);

            Debug.Log("Граната взорвалась");
            GrenadeAttackResult();
         
        }

        private void GrenadeAttackResult()
        {
            WManager.SetCurrentMassDamageWeapon(this);

            var characters = GManager.CharactersGroup.GetComponentsInChildren<CharacterController>();
            foreach (var enemy in characters)
            {
                if (enemy && Vector3.Distance(enemy.transform.position, transform.position) < _damageRadius)
                {
                    enemy.GetGrenadeDamage?.Invoke();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            grenadeFallDown = true;
        }

        public void UI_Effects(WeaponType type)
        {            
            switch (type)
            {
                case WeaponType.Fire:
                    StartCoroutine(ShootEffect());
                    break;
                case WeaponType.Throw:
                    StartCoroutine(Grenadeffect());                   
                    break;
                
            }        
        }
        public IEnumerator ShootEffect()
        {
            var effect = Effects.GetInstanceFxEffect();
            effect.SetActive(true);
            yield return new WaitForSeconds(.2f);
            Destroy(effect);
        }

        public IEnumerator Grenadeffect()
        {
            var effect = Effects.GetInstanceFxEffect();
            
            effect.SetActive(true);
            Effects.PlaySound("Bomb");

            while (effect.transform.localScale.magnitude < 8)
            {
                effect.transform.localScale *= 1 + 5 * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(.1f);
            Destroy(effect);
            foreach(var renderer in GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }
            yield return new WaitForSeconds(Effects.GeAaudioSource().clip.length);
            Destroy(gameObject);
        }
        
    }
}
