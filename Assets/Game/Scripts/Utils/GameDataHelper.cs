using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodymary.Game
{
    public static class GameDataHelper
    {
        public static int p_speed = Animator.StringToHash("Speed");
        public static int p_hit = Animator.StringToHash("IsHit");
        public static int p_hitting = Animator.StringToHash("IsHitting");
        public static int p_die = Animator.StringToHash("IsDie");
        public static int p_shoot = Animator.StringToHash("IsShoot");
        public static int p_shooting = Animator.StringToHash("IsShooting");
        public static int p_throwReady = Animator.StringToHash("ThrowReady");
        public static int p_throw = Animator.StringToHash("Throw");
        public static int p_throwCycle = Animator.StringToHash("ThrowCycle");
        public static int p_grenadeDamage = Animator.StringToHash("GrenadeDamage");

        public static string clipShootName = "m_pistol_shoot";
        public static string clipHitName = "m_melee_combat_attack_A";
        public static string clipIdleName = "m_idle_A";



    }
}

